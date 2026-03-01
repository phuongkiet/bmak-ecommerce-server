using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Commands.UpdateProductAttribute
{
    [AutoRegister]
    public class UpdateProductAttributeHandler : ICommandHandler<UpdateProductAttributeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductAttributeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateProductAttributeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Lấy Attribute kèm theo danh sách Options hiện tại từ DB
                var attribute = await _unitOfWork.Repository<ProductAttribute>()
                    .GetAllAsQueryable()
                    .Include(a => a.Values) // BẮT BUỘC Include để EF Core theo dõi bảng con
                    .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

                if (attribute == null)
                {
                    return Result<bool>.Failure($"Không tìm thấy thuộc tính với ID: {request.Id}");
                }

                // 2. Cập nhật thông tin cơ bản
                attribute.Name = request.Name;
                attribute.Code = request.Code;

                // 4. Lưu vào Database
                _unitOfWork.Repository<ProductAttribute>().Update(attribute);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Lỗi khi cập nhật thuộc tính: {ex.Message}");
            }
        }
    }
}
