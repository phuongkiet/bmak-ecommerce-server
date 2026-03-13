using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Categories.Admin.Common;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.Categories.Admin.Commands.CreateAdminCategory
{
    [AutoRegister]
    public class CreateAdminCategoryHandler : ICommandHandler<CreateAdminCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAdminCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateAdminCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var normalizedParentId = command.ParentId.HasValue && command.ParentId.Value > 0
                ? command.ParentId
                : null;

            if (normalizedParentId.HasValue)
            {
                var parent = await _unitOfWork.Repository<Category>().GetByIdAsync(normalizedParentId.Value);
                if (parent == null)
                {
                    return Result<int>.Failure("Danh muc cha khong ton tai.");
                }
            }

            var slug = CategorySlugHelper.GenerateSlug(command.Name);
            var duplicatedSlug = await _unitOfWork.Repository<Category>().FindAsync(x => x.Slug == slug);
            if (duplicatedSlug.Any())
            {
                return Result<int>.Failure($"Danh muc voi slug '{slug}' da ton tai.");
            }

            var category = new Category
            {
                Name = command.Name.Trim(),
                Description = command.Description?.Trim() ?? string.Empty,
                ParentId = normalizedParentId,
                Slug = slug
            };

            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(category.Id);
        }
    }
}
