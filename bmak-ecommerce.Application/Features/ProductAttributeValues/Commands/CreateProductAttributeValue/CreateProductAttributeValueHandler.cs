using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // 1. Validate ProductId tồn tại
            var product = await _unitOfWork.Repository<Product>()
                .GetByIdAsync(request.ProductId);
            
            if (product == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Product với ID: {request.ProductId}");
            }

            // 2. Validate AttributeId tồn tại
            var attribute = await _unitOfWork.Repository<ProductAttribute>()
                .GetByIdAsync(request.AttributeId);
            
            if (attribute == null)
            {
                throw new InvalidOperationException($"Không tìm thấy ProductAttribute với ID: {request.AttributeId}");
            }

            // 3. Kiểm tra xem Product đã có AttributeValue với AttributeId này chưa (tránh duplicate)
            // Một Product chỉ nên có một giá trị cho mỗi Attribute
            var existingValue = await _unitOfWork.Repository<ProductAttributeValue>()
                .FindAsync(x => x.ProductId == request.ProductId && x.AttributeId == request.AttributeId);
            
            if (existingValue.Any())
            {
                throw new InvalidOperationException(
                    $"Product (ID: {request.ProductId}) đã có giá trị cho Attribute (ID: {request.AttributeId}). " +
                    "Vui lòng cập nhật giá trị hiện có thay vì tạo mới.");
            }

            // 4. Map Command -> Entity
            var productAttributeValue = _mapper.Map<ProductAttributeValue>(request);

            // 5. Set giá trị mặc định cho ExtraData nếu null
            if (string.IsNullOrEmpty(productAttributeValue.ExtraData))
            {
                productAttributeValue.ExtraData = null; // Hoặc có thể set empty string tùy requirement
            }

            // 6. Lưu vào DB thông qua UnitOfWork
            await _unitOfWork.Repository<ProductAttributeValue>().AddAsync(productAttributeValue);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Trả về ID
            return productAttributeValue.Id;
        }
    }
}


