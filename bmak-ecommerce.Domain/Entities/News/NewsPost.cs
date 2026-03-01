using bmak_ecommerce.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.NewFolder
{
    public class NewsPost
    {
        public int Id { get; set; }

        // Thông tin phân loại và Tác giả
        public int CategoryId { get; set; }
        public NewsCategory Category { get; set; }

        public int? AuthorId { get; set; }
        public AppUser? Author { get; set; } // Người viết bài (Admin/Staff)

        // Nội dung chính
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty; // URL SEO (VD: /news/cach-chon-gach-op-lat)
        public string? Summary { get; set; } // Đoạn mô tả ngắn (Sapo)
        public string Content { get; set; } = string.Empty; // Chứa mã HTML từ Rich Text Editor (TinyMCE/CKEditor)
        public string? ThumbnailUrl { get; set; } // Ảnh bìa bài viết (Lưu link Cloudinary/Nethost)

        // Trạng thái & Thống kê
        public bool IsPublished { get; set; } = false; // Bản nháp (false) hay Đã xuất bản (true)
        public int ViewCount { get; set; } = 0; // Đếm lượt xem

        // Audit logs
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; } // Ngày giờ bấm nút xuất bản
    }
}
