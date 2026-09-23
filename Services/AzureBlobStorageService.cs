using System;
using System.Configuration;
using System.IO;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ContosoUniversity.Services
{
    /// <summary>
    /// Azure Blob Storage-backed implementation of <see cref="IBlobStorageService"/>. Authenticates
    /// with <see cref="DefaultAzureCredential"/> (Managed Identity in Azure, developer credentials
    /// locally) - no account keys or connection strings are used.
    ///
    /// The storage account name is read from the "AzureStorage:AccountName" application setting
    /// (see Web.config), never hardcoded. When Azure App Configuration (task 002) is reachable, the
    /// same key is sourced from there via the "AzureAppConfig" configuration builder.
    /// </summary>
    public class AzureBlobStorageService : IBlobStorageService
    {
        private const string DefaultContainerName = "teaching-materials";

        // Azure SDK clients (BlobServiceClient/BlobContainerClient/DefaultAzureCredential) are
        // thread-safe and designed to be reused. A single process-wide instance is shared across
        // every AzureBlobStorageService/CoursesController instance (each MVC request constructs a
        // new controller, but must not pay for a new client + token cache per request).
        private static readonly Lazy<BlobContainerClient> LazyContainerClient =
            new Lazy<BlobContainerClient>(CreateContainerClient);

        private static BlobContainerClient CreateContainerClient()
        {
            var accountName = ConfigurationManager.AppSettings["AzureStorage:AccountName"];
            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new InvalidOperationException(
                    "Missing required application setting 'AzureStorage:AccountName'. Set it to the " +
                    "provisioned Azure Storage account name (see infra/infra-config.md once the " +
                    "platform-engineer task has run) before uploading teaching material.");
            }

            var containerName = ConfigurationManager.AppSettings["AzureStorage:ContainerName"];
            if (string.IsNullOrWhiteSpace(containerName))
            {
                containerName = DefaultContainerName;
            }

            var serviceUri = new Uri($"https://{accountName}.blob.core.windows.net");
            var containerUri = new Uri(serviceUri, containerName);
            var containerClient = new BlobContainerClient(containerUri, new DefaultAzureCredential());

            // Idempotent create-if-not-exists - safe to call repeatedly (Rule 24: container
            // creation idempotency). Private container (no anonymous access).
            containerClient.CreateIfNotExists(PublicAccessType.None);
            return containerClient;
        }

        private static BlobContainerClient ContainerClient => LazyContainerClient.Value;

        public string UploadTeachingMaterial(int courseId, string fileExtension, Stream content, string contentType)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var blobName = $"course_{courseId}_{Guid.NewGuid()}{fileExtension}";
            var blobClient = ContainerClient.GetBlobClient(blobName);

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType
                }
                // MIGRATION NOTE: Conditions intentionally omitted -> unconditional overwrite. Each
                // upload targets a brand-new GUID-suffixed blob name (mirrors the original
                // SaveAs()-to-a-unique-filename semantics), so overwrite never actually occurs, but
                // this keeps the intent self-documenting per the "always succeed" original behavior.
            };

            blobClient.Upload(content, uploadOptions);

            return blobClient.Uri.AbsoluteUri;
        }

        public void DeleteTeachingMaterial(string teachingMaterialImagePath)
        {
            if (string.IsNullOrWhiteSpace(teachingMaterialImagePath))
            {
                return;
            }

            BlobClient blobClient;
            if (Uri.TryCreate(teachingMaterialImagePath, UriKind.Absolute, out var blobUri) &&
                (blobUri.Scheme == Uri.UriSchemeHttp || blobUri.Scheme == Uri.UriSchemeHttps))
            {
                // Stored reference is the absolute blob URL returned by UploadTeachingMaterial.
                blobClient = new BlobClient(blobUri, new DefaultAzureCredential());
            }
            else
            {
                // Backward compatibility: rows created before this migration may still contain a
                // local virtual path (e.g. "~/Uploads/TeachingMaterials/xyz.jpg"). There is nothing
                // to delete on local disk anymore, so fall back to treating the trailing file name
                // as a blob name in the current container.
                var blobName = Path.GetFileName(teachingMaterialImagePath.Replace('\\', '/'));
                if (string.IsNullOrWhiteSpace(blobName))
                {
                    return;
                }
                blobClient = ContainerClient.GetBlobClient(blobName);
            }

            try
            {
                blobClient.DeleteIfExists();
            }
            catch (RequestFailedException)
            {
                // Swallow deletion failures the same way the original implementation logged and
                // continued rather than blocking the course update/delete workflow.
            }
        }
    }
}
