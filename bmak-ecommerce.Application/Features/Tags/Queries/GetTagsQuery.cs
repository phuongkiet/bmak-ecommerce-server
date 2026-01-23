using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Tags.Queries
{
    public class GetTagsQuery : IRequest<List<TagDto>>
    {
        // Không cần params, chỉ lấy tất cả tags
    }
}


