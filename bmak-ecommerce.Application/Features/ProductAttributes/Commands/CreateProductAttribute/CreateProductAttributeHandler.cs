using AutoMapper;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Commands.CreateProductAttribute
{
    public class CreateProductAttributeHandler : IRequestHandler<CreateProductAttributeCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductAttributeHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateProductAttributeCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate Code phải unique (case-insensitive)
            var codeUpper = request.Code.ToUpperInvariant();
            var existingAttributes = await _unitOfWork.Repository<ProductAttribute>()
                .FindAsync(x => x.Code.ToUpper() == codeUpper);
            
            if (existingAttributes.Any())
            {
                throw new InvalidOperationException($"Đã tồn tại thuộc tính với mã Code: {request.Code}");
            }

            // 2. Map Command -> Entity
            var productAttribute = _mapper.Map<ProductAttribute>(request);

            // 3. Đảm bảo Code là UPPERCASE
            productAttribute.Code = request.Code.ToUpperInvariant();

            // 4. Lưu vào DB thông qua UnitOfWork
            await _unitOfWork.Repository<ProductAttribute>().AddAsync(productAttribute);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Trả về ID
            return productAttribute.Id;
        }
    }
}

