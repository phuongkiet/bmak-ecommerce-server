using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Media.DTOs;
using bmak_ecommerce.Domain.Entities.Media;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Media.Commands.UploadImage
{
    [AutoRegister]
    public class UploadImageHandler : ICommandHandler<UploadImageCommand, AppImageDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public UploadImageHandler(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<Result<AppImageDto>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            // 1. Upload lên ImgBB
            var uploadResult = await _imageService.UploadImageAsync(request.File);
            if (!uploadResult.IsSuccess) return Result<AppImageDto>.Failure(uploadResult.Error);

            // 2. Lưu Metadata vào DB (Media Library)
            var imageEntity = new AppImage
            {
                Url = uploadResult.Value.Url,
                PublicId = uploadResult.Value.PublicId,
                FileName = request.File.FileName,
                FileSize = request.File.Length,
                FileType = Path.GetExtension(request.File.FileName),
                AltText = request.AltText ?? request.File.FileName,
                CreatedAt = DateTime.UtcNow,
                Width = uploadResult.Value.Width,   // Lưu chiều rộng
                Height = uploadResult.Value.Height  // Lưu chiều cao
            };

            await _unitOfWork.Repository<AppImage>().AddAsync(imageEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Trả về DTO
            return Result<AppImageDto>.Success(new AppImageDto
            {
                Id = imageEntity.Id,
                Url = imageEntity.Url,
                FileName = imageEntity.FileName,
                AltText = imageEntity.AltText
            });
        }
    }
}
