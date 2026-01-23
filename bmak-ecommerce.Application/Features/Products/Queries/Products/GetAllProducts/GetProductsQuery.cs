using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts
{
    public class GetProductsQuery
    {
        public ProductSpecParams Params { get; set; }

        public GetProductsQuery(ProductSpecParams specParams)
        {
            Params = specParams;
        }
    }
}
