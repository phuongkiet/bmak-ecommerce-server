using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<PagedList<CategoryDto>>
    {
        public CategorySpecParams Params { get; set; }

        public GetCategoriesQuery(CategorySpecParams specParams)
        {
            Params = specParams;
        }
    }
}



