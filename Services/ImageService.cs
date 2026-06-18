using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Veloce.Exceptions;
using Veloco.Interfaces;

namespace Veloce.Services;

public class ImageService(Cloudinary cloudinary) : IImageService
{
    public async Task<string> UploadAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "veloce/cars",
            Transformation = new Transformation()
                .Width(1200)
                .Height(1200)
                .Crop("limit")
                .Quality("auto")
                .FetchFormat("auto")
        };

        var result = await cloudinary.UploadAsync(uploadParams);

        return result.Error != null ? 
            throw new AppException($"Image upload failed: {result.Error.Message}", 500) 
            : result.SecureUrl.ToString();
    }

    public async Task<List<string>> UploadMultipleAsync(List<IFormFile> files)
    {
        var tasks = files.Select(UploadAsync);
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    public async Task<bool> DeleteAsync(string imageUrl)
    {
        try
        {
            var publicId = ExtractPublicId(imageUrl);
            if (string.IsNullOrEmpty(publicId))
                return false;

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            var result = await cloudinary.DestroyAsync(deleteParams);
            return result.Error == null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteMultipleAsync(List<string> imageUrls)
    {
        var tasks = imageUrls.Select(DeleteAsync);
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }

    public string ExtractPublicId(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var segments = uri.Segments;

            var uploadIndex = Array.IndexOf(segments, "upload/");
            if (uploadIndex == -1)
                throw new AppException("Invalid Cloudinary URL format", 400);

            var publicIdWithVersion = string.Join("", segments.Skip(uploadIndex + 1));
            
            var parts = publicIdWithVersion.Split('/');
            if (parts.Length > 0 && parts[0].StartsWith("v"))
            {
                publicIdWithVersion = string.Join("/", parts.Skip(1));
            }
            
            var lastDotIndex = publicIdWithVersion.LastIndexOf('.');
            if (lastDotIndex > 0)
            {
                publicIdWithVersion = publicIdWithVersion[..lastDotIndex];
            }

            return publicIdWithVersion;
        }
        catch
        {
            return null;
        }
    }
}