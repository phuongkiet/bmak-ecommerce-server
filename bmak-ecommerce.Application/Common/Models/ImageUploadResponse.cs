using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Models
{
    public class ImageUploadResponse
    {
        public string Url { get; set; }
        public string PublicId { get; set; }
        public string Format { get; set; } // jpg, png, webp...
        public long Bytes { get; set; }    // Dung lượng (bytes)
        public int Width { get; set; }     // Chiều rộng (px)
        public int Height { get; set; }    // Chiều cao (px)
    }
}
