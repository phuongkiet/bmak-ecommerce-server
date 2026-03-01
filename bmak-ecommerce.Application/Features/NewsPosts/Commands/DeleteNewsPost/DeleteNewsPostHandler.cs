using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.NewsPosts.Commands.DeleteNewsPost
{
    [AutoRegister]
    public class DeleteNewsPostHandler : ICommandHandler<DeleteNewsPostCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNewsPostHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteNewsPostCommand command, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<NewsPost>();
            var post = await repo.GetByIdAsync(command.Id);

            if (post == null)
            {
                return Result<bool>.Failure("Không tìm thấy bài viết tin tức.");
            }

            repo.Remove(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
