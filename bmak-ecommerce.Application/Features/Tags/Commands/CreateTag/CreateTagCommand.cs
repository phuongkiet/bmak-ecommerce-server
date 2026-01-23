using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Tags.Commands.CreateTag
{
    public class CreateTagCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty; // VD: "Bán chạy", "Mới", "Khuyến mãi"
        public string? Description { get; set; } // Mô tả (optional)
    }
}


