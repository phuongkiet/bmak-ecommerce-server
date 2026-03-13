using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Categories.Admin.Common;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.UpdateAdminCategory
{
    [AutoRegister]
    public class UpdateAdminCategoryHandler : ICommandHandler<UpdateAdminCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAdminCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateAdminCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var categoryRepo = _unitOfWork.Repository<Category>();
            var category = await categoryRepo.GetByIdAsync(command.Id);
            if (category == null)
            {
                return Result<bool>.Failure("Khong tim thay danh muc.");
            }

            var normalizedParentId = command.ParentId.HasValue && command.ParentId.Value > 0
                ? command.ParentId
                : null;

            if (normalizedParentId == command.Id)
            {
                return Result<bool>.Failure("Khong the dat danh muc cha la chinh no.");
            }

            if (normalizedParentId.HasValue)
            {
                var parent = await categoryRepo.GetByIdAsync(normalizedParentId.Value);
                if (parent == null)
                {
                    return Result<bool>.Failure("Danh muc cha khong ton tai.");
                }

                if (await CreatesCircularReference(command.Id, normalizedParentId.Value, categoryRepo))
                {
                    return Result<bool>.Failure("Khong the cap nhat vi se tao vong lap danh muc.");
                }
            }

            var newSlug = CategorySlugHelper.GenerateSlug(command.Name);
            if (!string.Equals(category.Slug, newSlug, StringComparison.OrdinalIgnoreCase))
            {
                var existed = await categoryRepo.FindAsync(x => x.Slug == newSlug);
                if (existed.Any(x => x.Id != command.Id))
                {
                    return Result<bool>.Failure($"Danh muc voi slug '{newSlug}' da ton tai.");
                }
            }

            category.Name = command.Name.Trim();
            category.Description = command.Description?.Trim() ?? string.Empty;
            category.ParentId = normalizedParentId;
            category.Slug = newSlug;

            categoryRepo.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        private static async Task<bool> CreatesCircularReference(int categoryId, int candidateParentId, IGenericRepository<Category> categoryRepo)
        {
            var currentParentId = candidateParentId;

            while (true)
            {
                if (currentParentId == categoryId)
                {
                    return true;
                }

                var currentParent = await categoryRepo.GetByIdAsync(currentParentId);
                if (currentParent?.ParentId == null)
                {
                    return false;
                }

                currentParentId = currentParent.ParentId.Value;
            }
        }
    }
}
