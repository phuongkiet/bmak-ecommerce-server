using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bmak_ecommerce.Application.Common.Attributes;

namespace bmak_ecommerce.Infrastructure.Services.CloudinaryService
{
    [AutoRegister]
    public class CloudinaryService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            // 1. Lấy config từ appsettings.json
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            // 2. Khởi tạo Account & Cloudinary Instance
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Luôn dùng HTTPS
        }

        public async Task<Result<ImageUploadResponse>> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Result<ImageUploadResponse>.Failure("File không hợp lệ.");

            try
            {
                using var stream = file.OpenReadStream();

                // QUAN TRỌNG: Reset stream để tránh lỗi 0 byte như ImgBB
                if (stream.CanSeek) stream.Position = 0;

                // 3. Tạo tham số upload
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    // Tùy chọn: Folder trên Cloudinary để dễ quản lý
                    Folder = "bmak-products",
                    // Tùy chọn: Tự động nén ảnh ngay lúc upload (hoặc làm ở Frontend)
                    Transformation = new Transformation().Quality("auto")
                };

                // 4. Gọi SDK Upload (Hàm này chạy Async)
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null) return Result<ImageUploadResponse>.Failure("Lỗi khi upload ảnh.");

                return Result<ImageUploadResponse>.Success(new ImageUploadResponse
                {
                    Url = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId, // <--- Lấy ID ở đây
                    Format = uploadResult.Format,
                    Bytes = uploadResult.Bytes, // SDK trả về long
                    Width = uploadResult.Width, // SDK trả về int
                    Height = uploadResult.Height // SDK trả về int
                });
            }
            catch (Exception ex)
            {
                return Result<ImageUploadResponse>.Failure($"Lỗi Cloudinary: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result == "ok") return Result<bool>.Success(true);
            return Result<bool>.Failure("Delete failed on Cloudinary");
        }
    }
}
