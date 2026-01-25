using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.Commands.CreateOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface ICommandHandler<in TCommand, TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
    }
}
