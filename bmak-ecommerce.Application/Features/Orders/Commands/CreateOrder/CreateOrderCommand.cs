using bmak_ecommerce.Application.Features.Products.DTOs.Sale;

namespace bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand
    {
        public string CartId { get; set; }
        public string Note { get; set; }
        public string PaymentMethod { get; set; }

        // --- 1. THÔNG TIN NGƯỜI MUA / THANH TOÁN (Billing) ---
        // Dùng để liên hệ xác nhận đơn hoặc xuất hóa đơn
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerEmail { get; set; } // Thêm Email như ảnh
        public OrderAddressDto BillingAddress { get; set; }

        // --- 2. CỜ CHECKBOX ---
        // True: Giao đến địa chỉ khác
        // False: Giao đến địa chỉ của người mua (Billing)
        public bool ShipToDifferentAddress { get; set; }

        // --- 3. THÔNG TIN NGƯỜI NHẬN (Shipping) ---
        // Chỉ cần điền nếu ShipToDifferentAddress = true
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public OrderAddressDto? ShippingAddress { get; set; }
    }

    // Class con để gom nhóm địa chỉ cho gọn
    public class OrderAddressDto
    {
        public string Province { get; set; } // Tỉnh/Thành phố
        //public string District { get; set; } // Quận/Huyện
        public string Ward { get; set; }     // Phường/Xã
        public string SpecificAddress { get; set; } // Số nhà, tên đường

        public override string ToString()
        {
            //return $"{SpecificAddress}, {Ward}, {District}, {Province}";
            return $"{SpecificAddress}, {Ward}, {Province}";
        }
    }
}
