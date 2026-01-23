using bmak_ecommerce.Domain.Enums;

namespace bmak_ecommerce.Domain.Models
{
    public class OrderSpecParams
    {
        // --- 1. PHÂN TRANG (PAGINATION) ---
        private const int MaxPageSize = 50; // Chặn không cho client request quá nhiều gây sập DB
        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        // --- 2. TÌM KIẾM & SẮP XẾP ---
        public string? Sort { get; set; } // Ví dụ: "dateDesc", "priceAsc", "status"

        private string? _search;
        public string? Search
        {
            get => _search;
            set => _search = value?.ToLower().Trim(); // Chuẩn hóa về chữ thường để search không phân biệt hoa thường
        }

        // --- 3. BỘ LỌC NÂNG CAO (ADVANCED FILTERS) ---

        // Lọc theo Trạng thái (Quan trọng nhất với Admin)
        // Null = Lấy tất cả
        public OrderStatus? Status { get; set; }

        // Lọc theo User (Dùng khi xem lịch sử mua hàng của 1 khách cụ thể)
        public int? UserId { get; set; }

        // Lọc theo Mã đơn hàng (Tìm chính xác hoặc gần đúng)
        public string? OrderCode { get; set; }

        // Lọc theo khoảng thời gian (Reporting)
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Lọc theo phương thức thanh toán (Ít dùng hơn nhưng nên có)
        public PaymentMethod? PaymentMethod { get; set; }
    }
}