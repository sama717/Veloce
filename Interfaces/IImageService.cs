namespace Veloco.Interfaces;

public interface IImageService
{
    Task<string> UploadAsync(IFormFile file);
    Task<List<string>> UploadMultipleAsync(List<IFormFile> files);
    
    Task<bool> DeleteAsync(string imageUrl);
    Task<bool> DeleteMultipleAsync(List<string> imageUrls);
    
    string ExtractPublicId(string imageUrl);
}