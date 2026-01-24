using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Models
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 50;

        // FIX: Đổi PageIndex -> PageNumber để khớp với Repository và chuẩn API
        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Search { get; set; }

        // FIX: Thêm CategorySlug để lọc theo Slug (SEO friendly URL)
        public string? CategorySlug { get; set; }
        public int? CategoryId { get; set; } // Giữ lại nếu cần lọc nội bộ theo ID

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; }

        // --- FILTER ATTRIBUTES (DYNAMIC) ---
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Brand { get; set; }

        // --- FILTER TAGS ---
        private List<int>? _tagIds;
        public string? TagIdsString { get; set; }

        public List<int>? TagIds
        {
            get
            {
                if ((_tagIds == null || !_tagIds.Any()) && !string.IsNullOrEmpty(TagIdsString))
                {
                    _tagIds = TagIdsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                        .Where(id => id.HasValue)
                        .Select(id => id!.Value)
                        .ToList();
                }
                return _tagIds;
            }
            set => _tagIds = value;
        }
    }
}