using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Media.DTOs
{
    public class AppImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string PublicId { get; set; }
        public string AltText { get; set; }
        public long FileSize { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
