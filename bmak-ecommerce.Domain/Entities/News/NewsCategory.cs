using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.NewFolder
{
    public class NewsCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Slug dùng để làm URL thân thiện SEO (VD: /news/khuyen-mai)
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Quan hệ 1-N: 1 Danh mục có nhiều Bài viết
        public ICollection<NewsPost> Posts { get; set; } = new List<NewsPost>();
    }
}
