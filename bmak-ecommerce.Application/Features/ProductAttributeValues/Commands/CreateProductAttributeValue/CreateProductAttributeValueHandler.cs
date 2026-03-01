using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.CreateProductAttributeValue
{
    [AutoRegister]

    public class CreateProductAttributeValueHandler : IRequestHandler<CreateProductAttributeValueCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductAttributeValueHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate AttributeId tồn tại
            var attribute = await _unitOfWork.Repository<ProductAttribute>()
                .GetByIdAsync(request.AttributeId);

            if (attribute == null)
            {
                throw new InvalidOperationException($"Không tìm thấy ProductAttribute với ID: {request.AttributeId}");
            }

            // 2. Kiểm tra trùng value trong cùng attribute
            var normalizedValue = request.Value.Trim();
            var existingValue = await _unitOfWork.Repository<ProductAttributeValue>()
                .GetAllAsQueryable()
                .AnyAsync(x => x.AttributeId == request.AttributeId && x.Value == normalizedValue, cancellationToken);

            if (existingValue)
            {
                throw new InvalidOperationException(
                    $"Giá trị '{normalizedValue}' đã tồn tại trong Attribute (ID: {request.AttributeId}).");
            }

            // 3. Map Command -> Entity
            var productAttributeValue = _mapper.Map<ProductAttributeValue>(request);
            productAttributeValue.Value = normalizedValue;

            // 4. Set giá trị mặc định cho ExtraData nếu null
            if (string.IsNullOrEmpty(productAttributeValue.ExtraData))
            {
                productAttributeValue.ExtraData = null; // Hoặc có thể set empty string tùy requirement
            }

            // 5. Lưu vào DB thông qua UnitOfWork
            await _unitOfWork.Repository<ProductAttributeValue>().AddAsync(productAttributeValue);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Trả về ID
            return productAttributeValue.Id;
        }
    }
}


