using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.DeleteProductAttributeValue
{
    [AutoRegister]
    public class DeleteProductAttributeHandler : ICommandHandler<DeleteProductAttributeValueCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductAttributeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var repository = _unitOfWork.Repository<ProductAttributeValue>();
                var attributeValue = await repository.GetByIdAsync(request.Id);

                if (attributeValue == null)
                {
                    return Result<bool>.Failure($"Không tìm thấy ProductAttributeValue với ID: {request.Id}");
                }

                var isInUse = await _unitOfWork.Repository<ProductAttributeSelection>()
                    .GetAllAsQueryable()
                    .AnyAsync(x => x.AttributeValueId == request.Id, cancellationToken);

                if (isInUse)
                {
                    return Result<bool>.Failure("Không thể xóa giá trị thuộc tính vì đang được gán cho sản phẩm");
                }

                repository.Remove(attributeValue);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Lỗi khi xóa ProductAttributeValue: {ex.Message}");
            }
        }
    }
}
