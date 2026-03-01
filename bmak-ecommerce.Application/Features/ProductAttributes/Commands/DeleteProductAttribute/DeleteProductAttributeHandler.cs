using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Commands.DeleteProductAttribute
{
    [AutoRegister]
    public class DeleteProductAttributeHandler : ICommandHandler<DeleteProductAttributeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductAttributeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteProductAttributeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Lấy Attribute từ DB, bắt buộc phải dùng Include để lấy kèm danh sách các Values/Options
                var attribute = await _unitOfWork.Repository<ProductAttribute>()
                    .GetAllAsQueryable()
                    .Include(a => a.Values) // Load các giá trị con lên
                    .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

                if (attribute == null)
                {
                    return Result<bool>.Failure($"Không tìm thấy thuộc tính với ID: {request.Id}");
                }

                // 2. CHECK RÀNG BUỘC: Nếu có Value (Option) thì KHÔNG CHO XÓA
                if (attribute.Values != null && attribute.Values.Any())
                {
                    return Result<bool>.Failure($"Không thể xóa thuộc tính '{attribute.Name}' vì đang chứa {attribute.Values.Count} giá trị. Vui lòng xóa các giá trị con trước!");
                }

                // (Tùy chọn thêm) 3. CHECK RÀNG BUỘC: Nếu thuộc tính đang được gắn cho Sản Phẩm nào đó thì cũng không cho xóa.
                // Nếu bạn có bảng ProductAttributeValue, bạn có thể check thêm ở đây để web không bị lỗi dữ liệu sản phẩm.

                // 4. Nếu vượt qua bài test ràng buộc -> Tiến hành Xóa
                _unitOfWork.Repository<ProductAttribute>().Remove(attribute);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Lỗi khi xóa thuộc tính: {ex.Message}");
            }
        }
    }
}
