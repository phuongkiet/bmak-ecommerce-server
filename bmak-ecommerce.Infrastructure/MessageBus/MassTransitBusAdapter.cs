using bmak_ecommerce.Application.Common.Interfaces; // Nơi chứa interface IMessageBus
using MassTransit;
using System.Threading;

namespace bmak_ecommerce.Infrastructure.MessageBus
{
    // Class này đóng vai trò "Cầu nối" (Adapter)
    // Giúp Application Layer không cần biết MassTransit là gì, chỉ biết IMessageBus
    public class MassTransitBusAdapter : IMessageBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        // MassTransit tự động Inject IPublishEndpoint vào đây
        public MassTransitBusAdapter(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }
    }
}