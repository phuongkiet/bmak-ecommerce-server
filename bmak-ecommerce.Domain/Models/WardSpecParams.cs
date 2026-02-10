using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Models
{
    public class WardSpecParams
    {
        private const int MaxPageSize = 10000;
        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string ProvinceId { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; }
    }
}
