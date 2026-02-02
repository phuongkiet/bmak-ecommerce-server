using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommand
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; } 
        public PageStatusType Status { get; set; }
    }
}
