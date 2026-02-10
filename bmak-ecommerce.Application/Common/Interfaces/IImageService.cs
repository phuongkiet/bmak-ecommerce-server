using bmak_ecommerce.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Upload file lên Server ảnh (ImgBB)
        /// </summary>
        /// <param name="file">File từ Form</param>
        /// <returns>URL ảnh (hoặc lỗi)</returns>
        Task<Result<ImageUploadResponse>> UploadImageAsync(IFormFile file);
        Task<Result<bool>> DeleteImageAsync(string publicId);
    }
}
