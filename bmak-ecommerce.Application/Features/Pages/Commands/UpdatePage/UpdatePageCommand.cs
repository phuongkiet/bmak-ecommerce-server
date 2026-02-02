using bmak_ecommerce.Application.Features.Pages.DTOs;
using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommand
    {
        public int Id { get; set; } // Ẩn trong route
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }

        // Thay đổi quan trọng: Nhận List Object thay vì String
        public List<PageSectionDto> Sections { get; set; }

        public PageStatusType IsPublished { get; set; }
    }
}
