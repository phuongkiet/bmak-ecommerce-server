using AutoMapper;
using bmak_ecommerce.Application.Features.Products.DTOs.Sale;
using bmak_ecommerce.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Mappings
{
    public class OrderMappingConfiguration : Profile
    {
        public OrderMappingConfiguration() {
            // =========================================================
            // 1. MAPPING CHO TRANG DANH SÁCH (Admin Grid / History)
            // =========================================================
            CreateMap<Order, OrderSummaryDto>()
                // Convert Enum sang String để Frontend dễ hiển thị
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))

                // Lấy tên khách hàng từ bảng User (Flattening)
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))

                // Đếm số lượng sản phẩm thay vì load hết list
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));


            // =========================================================
            // 2. MAPPING CHO TRANG CHI TIẾT (Detail View)
            // =========================================================
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))

                // Map thông tin khách hàng
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.User.PhoneNumber))

                // Map địa chỉ giao hàng (xử lý null nếu khách nhận tại kho)
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src =>
                    src.ShippingAddress != null
                    ? $"{src.ShippingAddress.Street}, {src.ShippingAddress.City}" // Format tùy ý bạn
                    : "Nhận tại cửa hàng/Kho"));

            // Mapping chi tiết từng món hàng
            CreateMap<OrderItem, OrderItemDto>()
                // Nếu OrderItem chưa lưu tên/ảnh snapshot, có thể map từ Product (nếu có Include)
                // .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrl)) 
                .ForMember(dest => dest.QuantitySquareMeter, opt => opt.MapFrom(src => src.QuantitySquareMeter))
                ;
        }
    }
}
