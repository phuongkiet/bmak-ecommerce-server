using bmak_ecommerce.Application.Common.Models;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface ICommandHandler<in TCommand, TResult>
    {
        Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken = default);
    }
}
