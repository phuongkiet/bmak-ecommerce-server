using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Models
{
    public class CategorySpecParams
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
        public int? ParentId { get; set; } // Lọc theo danh mục cha (null = chỉ lấy danh mục gốc)
        public string? Sort { get; set; } // "nameAsc", "nameDesc", "productCountDesc"
        public bool? IncludeOnlyRootCategories { get; set; } // Chỉ lấy danh mục gốc (không có parent)
    }
}



