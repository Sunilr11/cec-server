using Microsoft.AspNetCore.Http;

namespace GDT.CEC.Service.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageToBlobAsync(IFormFile imageFile);
        Task<string> UploadImageToBlobAsync(byte[] bytes, string filename, string contentType);
    }
}