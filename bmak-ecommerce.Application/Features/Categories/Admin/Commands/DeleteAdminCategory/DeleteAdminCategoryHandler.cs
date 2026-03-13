using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.DeleteAdminCategory
{
    [AutoRegister]
    public class DeleteAdminCategoryHandler : ICommandHandler<DeleteAdminCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public DeleteAdminCategoryHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<bool>> Handle(DeleteAdminCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var categoryRepo = _unitOfWork.Repository<Category>();
            var category = await categoryRepo.GetByIdAsync(command.Id);
            if (category == null)
            {
                return Result<bool>.Failure("Khong tim thay danh muc.");
            }

            var hasChildren = (await categoryRepo.FindAsync(x => x.ParentId == command.Id)).Any();
            if (hasChildren)
            {
                return Result<bool>.Failure("Khong the xoa danh muc dang co danh muc con.");
            }

            var categoryDetail = await _categoryRepository.GetCategoryDetailAsync(command.Id);
            if (categoryDetail != null && categoryDetail.ProductCategories.Any())
            {
                return Result<bool>.Failure("Khong the xoa danh muc da duoc gan san pham.");
            }

            categoryRepo.Remove(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
