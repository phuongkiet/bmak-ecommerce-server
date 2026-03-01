using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.ProductAttributeValues.DTOs;
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.Queries.GetAttributeValuesQuery
{
    [AutoRegister]
    public class GetAttributeValuesHandler : IQueryHandler<GetAttributeValuesQuery, List<ProductAttributeValueDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAttributeValuesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ProductAttributeValueDto>>> Handle(GetAttributeValuesQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy dữ liệu từ Repository
            var entities = await _unitOfWork.Repository<ProductAttributeValue>()
                .GetAllAsQueryable()
                .Where(x => x.AttributeId == request.AttributeId)
                .ToListAsync(cancellationToken);

            // 2. Map từ Entity sang DTO (Pure CQRS thường map thủ công hoặc dùng Select)
            var dtos = entities.Select(x => new ProductAttributeValueDto
            {
                Id = x.Id,
                Value = x.Value,
                ExtraData = x.ExtraData,
                AttributeId = x.AttributeId
                // Map thêm các trường khác nếu cần
            }).ToList();

            return Result<List<ProductAttributeValueDto>>.Success(dtos);
        }
    }
}
