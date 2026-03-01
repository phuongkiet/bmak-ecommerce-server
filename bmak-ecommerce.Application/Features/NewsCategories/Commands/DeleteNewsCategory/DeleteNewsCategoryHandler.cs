using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.NewFolder;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.NewsCategories.Commands.DeleteNewsCategory
{
    [AutoRegister]
    public class DeleteNewsCategoryHandler : ICommandHandler<DeleteNewsCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNewsCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteNewsCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var categoryRepo = _unitOfWork.Repository<NewsCategory>();
            var category = await categoryRepo.GetByIdAsync(command.Id);

            if (category == null)
            {
                return Result<bool>.Failure("Không tìm thấy danh mục tin tức.");
            }

            var hasPosts = await _unitOfWork.Repository<NewsPost>().FindAsync(x => x.CategoryId == command.Id);
            if (hasPosts.Any())
            {
                return Result<bool>.Failure("Danh mục đang có bài viết, không thể xóa.");
            }

            categoryRepo.Remove(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
