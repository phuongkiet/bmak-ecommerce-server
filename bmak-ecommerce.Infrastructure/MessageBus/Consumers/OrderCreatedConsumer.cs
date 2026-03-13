using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Infrastructure.SignalR;

// using bmak_ecommerce.Infrastructure.SignalR; // Nếu em đã setup SignalR
using MassTransit;
using Microsoft.AspNetCore.SignalR;

// using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace bmak_ecommerce.Infrastructure.MessageBus.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IEmailService _emailService;
        private readonly IHubContext<AdminNotificationHub> _hubContext;

        public OrderCreatedConsumer(
            ILogger<OrderCreatedConsumer> logger,
            IEmailService emailService,
             IHubContext<AdminNotificationHub> hubContext
            )
        {
            _logger = logger;
            _emailService = emailService;
             _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation($"[RabbitMQ] Bắt đầu xử lý đơn: {message.OrderCode} - ID: {message.OrderId}");

            try
            {
                // 1. TẠO HTML DANH SÁCH SẢN PHẨM ĐỘNG TRƯỚC
                string itemsHtml = string.Join("", message.Items.Select(item => $@"
                    <tr>
                        <td style='padding: 12px 0; border-bottom: 1px dashed #e2e8f0;'>
                            <span style='color: #0f172a; font-size: 14px;'>{item.ProductName}</span>
                        </td>
                        <td align='center' style='padding: 12px 0; border-bottom: 1px dashed #e2e8f0;'>
                            <span style='color: #475569; font-size: 14px;'>{item.Quantity}</span>
                        </td>
                        <td align='right' style='padding: 12px 0; border-bottom: 1px dashed #e2e8f0;'>
                            <span style='color: #0f172a; font-size: 14px; font-weight: bold;'>{item.TotalPrice:N0} đ</span>
                        </td>
                    </tr>
                "));

                // 2. NHÚNG VÀO EMAIL GỐC
                string customerHtmlContent = $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head><meta charset='UTF-8'></head>
                <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                    <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>
                        
                        <tr>
                            <td align='center' style='background-color: #2563eb; padding: 30px 20px;'>
                                <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>BMAK STORE</h1>
                                <p style='color: #bfdbfe; margin: 10px 0 0 0; font-size: 16px;'>Cảm ơn bạn đã đặt hàng!</p>
                            </td>
                        </tr>

                        <tr>
                            <td style='padding: 30px;'>
                                <p style='color: #333333; font-size: 16px; margin-bottom: 20px;'>Xin chào,</p>
                                <p style='color: #333333; font-size: 16px; margin-bottom: 30px;'>Đơn hàng của bạn đã được hệ thống ghi nhận. Dưới đây là chi tiết đơn hàng:</p>

                                <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom: 20px;'>
                                    <tr>
                                        <td width='50%' style='padding-bottom: 15px;'>
                                            <strong style='color: #475569; font-size: 13px; text-transform: uppercase;'>Mã đơn hàng:</strong><br>
                                            <span style='color: #2563eb; font-size: 18px; font-weight: bold;'>{message.OrderCode}</span>
                                        </td>
                                        <td width='50%' style='padding-bottom: 15px; text-align: right;'>
                                            <strong style='color: #475569; font-size: 13px; text-transform: uppercase;'>Ngày đặt:</strong><br>
                                            <span style='color: #0f172a; font-size: 16px;'>{message.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}</span>
                                        </td>
                                    </tr>
                                </table>

                                <table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 15px; margin-bottom: 30px;'>
                                    <thead>
                                        <tr>
                                            <th align='left' style='padding-bottom: 10px; border-bottom: 2px solid #e2e8f0; color: #475569; font-size: 13px;'>Sản phẩm</th>
                                            <th align='center' style='padding-bottom: 10px; border-bottom: 2px solid #e2e8f0; color: #475569; font-size: 13px;'>SL</th>
                                            <th align='right' style='padding-bottom: 10px; border-bottom: 2px solid #e2e8f0; color: #475569; font-size: 13px;'>Thành tiền</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {itemsHtml}
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <td colspan='2' align='right' style='padding-top: 15px;'>
                                                <strong style='color: #475569; font-size: 16px;'>Tổng thanh toán:</strong>
                                            </td>
                                            <td align='right' style='padding-top: 15px;'>
                                                <span style='color: #dc2626; font-size: 20px; font-weight: bold;'>{message.TotalAmount:N0} đ</span>
                                            </td>
                                        </tr>
                                    </tfoot>
                                </table>

                                <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                    <tr>
                                        <td align='center'>
                                            <a href='https://bmak.com/orders/{message.OrderCode}' style='background-color: #2563eb; color: #ffffff; text-decoration: none; padding: 12px 24px; border-radius: 6px; font-weight: bold; display: inline-block;'>Tra cứu đơn hàng</a>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td align='center' style='background-color: #f8fafc; padding: 20px; border-top: 1px solid #e2e8f0;'>
                                            <p style='color: #64748b; font-size: 14px; margin: 0 0 10px 0;'>
                                                Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ bộ phận CSKH:
                                            </p>
                                            <p style='color: #2563eb; font-size: 14px; font-weight: bold; margin: 0;'>
                                                Hotline: 1900 xxxx | Email: support@bmak.com
                                            </p>
                                            <p style='color: #94a3b8; font-size: 12px; margin: 15px 0 0 0;'>
                                                © {DateTime.Now.Year} BMAK Store. All rights reserved.
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                // 2. GỬI EMAIL CHO KHÁCH HÀNG
                if (!string.IsNullOrEmpty(message.CustomerEmail))
                {
                    await _emailService.SendEmailAsync(
                        toEmail: message.CustomerEmail,
                        subject: $"Xác nhận đơn hàng #{message.OrderCode} từ BMAK Store",
                        htmlContent: customerHtmlContent
                    );
                }

                // 3. (Tùy chọn) TẠO HTML NGẮN GỌN GỬI CHO ADMIN SALE
                string adminHtmlContent = $"<h3>Có đơn hàng mới!</h3><p>Mã đơn: <b>{message.OrderCode}</b></p><p>Tổng tiền: <b style='color:red'>{message.TotalAmount:N0} VNĐ</b></p>";
                await _emailService.SendEmailAsync("saleadmin@bmak.com", $"Đơn hàng mới: {message.OrderCode}", adminHtmlContent);

                // 4. Bắn SignalR (Nếu có)
                 await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", new
                 {
                     OrderCode = message.OrderCode,
                     TotalAmount = message.TotalAmount,
                     Time = message.CreatedAt,
                     Message = $"Khách hàng vừa đặt đơn {message.OrderCode} trị giá {message.TotalAmount:N0}đ!"
                 });

                _logger.LogInformation($"[RabbitMQ] Đã xử lý xong đơn và gửi email: {message.OrderCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[RabbitMQ] Lỗi khi xử lý đơn {message.OrderCode}");
                throw;
            }
        }
    }
}