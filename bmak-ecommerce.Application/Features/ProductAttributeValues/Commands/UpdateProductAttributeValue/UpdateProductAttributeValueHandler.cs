using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.UpdateProductAttributeValue
{
    [AutoRegister]
    public class UpdateProductAttributeValueHandler : ICommandHandler<UpdateProductAttributeValueCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductAttributeValueHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var attributeValueRepository = _unitOfWork.Repository<ProductAttributeValue>();

                var attributeValue = await attributeValueRepository.GetByIdAsync(request.Id);
                if (attributeValue == null)
                {
                    return Result<bool>.Failure($"Không tìm thấy ProductAttributeValue với ID: {request.Id}");
                }

                var attribute = await _unitOfWork.Repository<ProductAttribute>().GetByIdAsync(request.AttributeId);
                if (attribute == null)
                {
                    return Result<bool>.Failure($"Không tìm thấy ProductAttribute với ID: {request.AttributeId}");
                }

                var normalizedValue = request.Value.Trim();
                var duplicate = await attributeValueRepository
                    .GetAllAsQueryable()
                    .AnyAsync(x =>
                        x.AttributeId == request.AttributeId &&
                        x.Value == normalizedValue &&
                        x.Id != request.Id,
                        cancellationToken);

                if (duplicate)
                {
                    return Result<bool>.Failure(
                        $"Giá trị '{normalizedValue}' đã tồn tại trong Attribute (ID: {request.AttributeId}).");
                }

                attributeValue.Value = normalizedValue;
                attributeValue.ExtraData = string.IsNullOrWhiteSpace(request.ExtraData) ? null : request.ExtraData;
                attributeValue.AttributeId = request.AttributeId;

                attributeValueRepository.Update(attributeValue);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Lỗi khi cập nhật ProductAttributeValue: {ex.Message}");
            }
        }
    }
}
