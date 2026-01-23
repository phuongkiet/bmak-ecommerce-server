using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetTopSellingProduct
{
    public record GetTopSellingProductsQuery : IRequest<List<ProductDto>>;
}
