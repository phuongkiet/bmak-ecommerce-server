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
        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Search { get; set; } // Tìm theo tên
        public int? CategoryId { get; set; } // Lọc theo danh mục
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; } // "priceAsc", "priceDesc"

        // Lọc nâng cao: VD "size:60x60,color:grey"
        // Frontend sẽ gửi chuỗi: ?attributes=size:60x60,color:grey
        public string? Attributes { get; set; }

        // Lọc theo Tag IDs 
        // Có thể filter theo nhiều tags (AND logic - products phải có TẤT CẢ các tags)
        // Format 1: ?tagIds=1&tagIds=2&tagIds=3 (ASP.NET Core tự bind List<int>)
        // Format 2: ?tagIds=1,2,3 (parse từ TagIdsString)
        private List<int>? _tagIds;
        
        // Property này sẽ được bind từ query string
        public List<int>? TagIds 
        { 
            get 
            {
                // Nếu TagIds chưa được set nhưng TagIdsString có giá trị, parse từ TagIdsString
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

        // Helper property để bind từ query string dạng comma-separated: ?tagIds=1,2,3
        public string? TagIdsString { get; set; }
    }
}
