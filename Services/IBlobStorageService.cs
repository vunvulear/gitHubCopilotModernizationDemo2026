using System.IO;

namespace ContosoUniversity.Services
{
    /// <summary>
    /// Abstraction over Azure Blob Storage used to persist and remove course teaching-material
    /// images. Replaces the previous local-disk ("~/Uploads/TeachingMaterials/") storage so the
    /// application does not depend on filesystem paths that are unavailable/ephemeral when
    /// hosted in Azure.
    /// </summary>
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads teaching material content to Blob Storage and returns the absolute blob URL to
        /// persist as <c>Course.TeachingMaterialImagePath</c>.
        /// </summary>
        /// <param name="courseId">Course identifier, used only to build a human-readable blob name.</param>
        /// <param name="fileExtension">File extension (including the leading dot), e.g. ".jpg".</param>
        /// <param name="content">The posted file content stream. Read once, from its current position.</param>
        /// <param name="contentType">The posted file's content type, stored as the blob's HTTP Content-Type.</param>
        /// <returns>The absolute HTTPS URL of the uploaded blob.</returns>
        string UploadTeachingMaterial(int courseId, string fileExtension, Stream content, string contentType);

        /// <summary>
        /// Deletes a previously uploaded teaching material blob given the stored reference (the
        /// absolute blob URL previously returned by <see cref="UploadTeachingMaterial"/>).
        /// No-ops when the reference is null/empty or the blob no longer exists.
        /// </summary>
        void DeleteTeachingMaterial(string teachingMaterialImagePath);
    }
}
