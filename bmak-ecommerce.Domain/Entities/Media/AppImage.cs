using bmak_ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Media
{
    public class AppImage : BaseEntity
    {
        public string FileName { get; set; }
        public string Url { get; set; } // Link ImgBB
        public string? DeleteUrl { get; set; } // Link xóa của ImgBB (nếu cần)
        public string FileType { get; set; } // .jpg, .png
        public long FileSize { get; set; } // bytes
        public string PublicId { get; set; }
        public int Width { get; set; }  // <--- Thêm cột này
        public int Height { get; set; } // <--- Thêm cột này

        // Metadata (như WordPress)
        public string? AltText { get; set; }
        public string? Title { get; set; }
    }
}
