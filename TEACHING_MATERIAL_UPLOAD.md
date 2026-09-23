# Teaching Material Image Upload Feature

This feature allows administrators to upload images for teaching materials (textbooks) associated with courses.

## Features

- **Image Upload**: Upload teaching material images when creating or editing courses
- **File Validation**: Supports JPG, JPEG, PNG, GIF, and BMP formats
- **Size Limits**: Maximum file size of 5MB per image
- **Secure Storage**: Images are stored in Azure Blob Storage (private container, Managed Identity authentication)
- **Automatic Cleanup**: Images are automatically deleted when courses are removed
- **Unique Filenames**: Each uploaded image gets a unique blob name to prevent conflicts

## Usage

### Creating a Course with Teaching Material Image

1. Navigate to the Courses section
2. Click "Create New" (Admin only)
3. Fill in the course details
4. In the "Teaching Material Image" section, click "Choose File"
5. Select an image file (JPG, JPEG, PNG, GIF, or BMP)
6. Click "Create" to save the course

### Editing a Course's Teaching Material Image

1. Navigate to the Courses section
2. Click "Edit" next to the course you want to modify
3. If a teaching material image already exists, it will be displayed
4. To change the image, click "Choose File" and select a new image
5. Click "Save" to update the course

### Viewing Teaching Material Images

- **Course List**: Small thumbnails (50x50px) are displayed in the courses index
- **Course Details**: Full-size images (max 300x300px) are displayed on the course details page

## Technical Details

### File Storage
- Images are uploaded to an Azure Blob Storage container (default name `teaching-materials`) via
  `ContosoUniversity.Services.AzureBlobStorageService` (`Azure.Storage.Blobs` SDK)
- Authentication uses Azure Managed Identity (`DefaultAzureCredential`) - no account keys or
  connection strings are stored in configuration
- Blob names follow the pattern: `course_{CourseID}_{GUID}.{extension}`
- Old blobs are automatically deleted when replaced (the new blob is uploaded first, then the
  previous one is removed, so a failed upload never destroys the existing image)
- The storage account name is configured via the `AzureStorage:AccountName` application setting
  (see `Web.config` / Azure App Configuration) - the blob endpoint is constructed at runtime as
  `https://{AzureStorage:AccountName}.blob.core.windows.net`

### Database Schema
- Field: `TeachingMaterialImagePath` (VARCHAR(255)) on the Course table
- Stores the absolute Azure Blob Storage URL of the uploaded image (previously stored a relative
  local-disk virtual path such as `~/Uploads/TeachingMaterials/xyz.jpg`)

### Security
- File type validation prevents uploading of non-image files
- File size validation prevents uploads larger than 5MB
- Only authenticated users with appropriate roles can upload images
- The blob container is created with private (`PublicAccessType.None`) access; only identities
  granted an appropriate Azure RBAC role (e.g. `Storage Blob Data Contributor`) can read or write

### Authorization
- **Create/Upload**: Admin role required
- **Edit/Upload**: Admin or Teacher role required
- **View**: All authenticated users can view images
- **Delete**: Admin role required (deletes both course and associated blob)

## Troubleshooting

### Common Issues

1. **"File too large" error**: Ensure your image is under 5MB
2. **"Invalid file type" error**: Only JPG, JPEG, PNG, GIF, and BMP files are supported
3. **Upload fails / `InvalidOperationException` about `AzureStorage:AccountName`**: Ensure the
   `AzureStorage:AccountName` application setting has been set to the provisioned storage account
   name (see `infra/infra-config.md` once the platform-engineer task has provisioned it)
4. **Upload fails with 401/403**: Ensure the app's Managed Identity has been granted the
   `Storage Blob Data Contributor` role on the target storage account/container

### Configuration

The following settings in `Web.config` control file upload limits:
- `maxRequestLength="10240"` (10MB in KB)
- `maxAllowedContentLength="10485760"` (10MB in bytes)
- `executionTimeout="3600"` (1 hour timeout for large uploads)

The following settings in `Web.config` control the Azure Blob Storage destination:
- `AzureStorage:AccountName` - the storage account hosting teaching-material blobs
- `AzureStorage:ContainerName` - the blob container name (default `teaching-materials`)

## Deployment Considerations

### Initial Setup
1. Provision an Azure Storage account and grant the application's Managed Identity the
   `Storage Blob Data Contributor` role on it (the application creates the `teaching-materials`
   container automatically on first use via `CreateIfNotExists`)
2. Set the `AzureStorage:AccountName` application setting (directly, or via Azure App Configuration)
   to the provisioned storage account name

### Backup Strategy
- Configure standard Azure Storage data protection features (soft delete, versioning, or
  geo-redundant storage) for the `teaching-materials` container as appropriate for your
  environment

## Future Enhancements

Potential improvements for this feature:
- Image resizing and optimization
- Multiple image support per course
- Image gallery view
- Bulk upload functionality
- Image metadata support (alt text, captions)

