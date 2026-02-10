using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Queries
{
    [AutoRegister]

    public class GetProductAttributesHandler : IRequestHandler<GetProductAttributesQuery, List<ProductAttributeListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductAttributesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ProductAttributeListItemDto>> Handle(GetProductAttributesQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy tất cả ProductAttributes từ repository
            var attributes = await _unitOfWork.Repository<ProductAttribute>().GetAllAsync();

            // 2. Map Entity -> DTO
            var attributeDtos = _mapper.Map<List<ProductAttributeListItemDto>>(attributes);

            // 3. Trả về danh sách attributes
            return attributeDtos;
        }
    }
}


