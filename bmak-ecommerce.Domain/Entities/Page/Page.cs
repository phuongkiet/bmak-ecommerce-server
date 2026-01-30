using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Page
{
    public class Page
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ContentJson { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
