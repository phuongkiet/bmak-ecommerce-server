using bmak_ecommerce.Domain.Common;
using bmak_ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Entities.Sales
{
    public class Voucher : BaseEntity
    {
        // Mã code khách hàng sẽ nhập (VD: BMAK2026, FREESHIP)
        public string Code { get; set; } = string.Empty;

        // Tên hiển thị/Mô tả (VD: "Giảm 50k cho đơn từ 500k")
        public string Description { get; set; } = string.Empty;

        // CẤU HÌNH GIẢM GIÁ
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; } // Nếu là FixedAmount thì đây là số tiền. Nếu là Percentage thì đây là % (VD: 10)

        // ĐIỀU KIỆN ÁP DỤNG
        public decimal MinOrderAmount { get; set; } = 0; // Đơn tối thiểu để được dùng (0 = Không giới hạn)

        // Mức giảm tối đa (Chỉ có tác dụng khi DiscountType = Percentage)
        // Ví dụ: Giảm 10% nhưng tối đa 100k -> MaxDiscountAmount = 100000
        public decimal? MaxDiscountAmount { get; set; }

        // GIỚI HẠN THỜI GIAN & SỐ LƯỢNG
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } // Ngày hết hạn

        public int UsageLimit { get; set; } = 0; // Tổng số lần mã này có thể được xài trên toàn hệ thống (0 = Vô hạn)
        public int UsedCount { get; set; } = 0;  // Số lần đã được khách xài thành công

        public int PerUserLimit { get; set; } = 1; // Mỗi User được xài mã này mấy lần (Thường là 1)

        // Trạng thái bật/tắt (Để Admin chủ động tắt mã khẩn cấp dù chưa hết hạn)
        public bool IsActive { get; set; } = true;

        // Hàm helper tự động check xem Voucher còn hợp lệ không
        public bool IsValid()
        {
            var now = DateTime.UtcNow;
            if (!IsActive) return false;
            if (now < StartDate || now > EndDate) return false;
            if (UsageLimit > 0 && UsedCount >= UsageLimit) return false;

            return true;
        }
    }
}
