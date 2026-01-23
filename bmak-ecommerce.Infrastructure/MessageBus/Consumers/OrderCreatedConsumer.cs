using bmak_ecommerce.Application.Common.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.MessageBus.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;
        // Sau này sẽ Inject IEmailService (Brevo) vào đây

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;
            // Log đúng tên biến
            _logger.LogInformation($"[RabbitMQ] Nhận sự kiện tạo đơn: {message.OrderCode} - ID: {message.OrderId}");

            // 2. Logic gửi Email sẽ nằm ở đây (TODO: Implement Brevo)
            // await _emailService.SendEmailAsync(message.CustomerEmail, ...);

            await Task.CompletedTask;
        }
    }
}