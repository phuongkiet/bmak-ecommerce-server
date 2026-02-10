using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Media;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Media.Commands.DeleteImage
{
    [AutoRegister] // Tự động đăng ký Scoped
    public class DeleteImageHandler : ICommandHandler<DeleteImageCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public DeleteImageHandler(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<Result<bool>> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm ảnh trong DB
            var imageRepo = _unitOfWork.Repository<AppImage>();
            var image = await imageRepo.GetByIdAsync(request.Id);

            if (image == null)
                return Result<bool>.Failure("Ảnh không tồn tại trong hệ thống.");

            // 2. Xóa trên Cloudinary trước (để đảm bảo sạch sẽ)
            // Lưu ý: Dù xóa trên mây thất bại, ta có thể chọn xóa luôn ở DB hoặc giữ lại tùy business.
            // Ở đây mình chọn: Phải xóa mây thành công mới xóa DB.
            if (!string.IsNullOrEmpty(image.PublicId))
            {
                var cloudResult = await _imageService.DeleteImageAsync(image.PublicId);
                if (!cloudResult.IsSuccess)
                {
                    return Result<bool>.Failure($"Không thể xóa ảnh trên Cloud: {cloudResult.Error}");
                }
            }

            // 3. Xóa trong Database
            imageRepo.Remove(image);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
