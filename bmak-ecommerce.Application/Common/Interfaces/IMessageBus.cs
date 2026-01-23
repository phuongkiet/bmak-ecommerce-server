using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface IMessageBus
    {
        Task PublishMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }
}
