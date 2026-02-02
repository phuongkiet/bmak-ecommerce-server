using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Page;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Commands.CreatePage
{
    public class CreatePageHandler : ICommandHandler<CreatePageCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(CreatePageCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate trùng lặp (nếu cần)

            // 2. Tự sinh Slug nếu user không nhập, hoặc chuẩn hóa Slug user nhập
            // Ví dụ: "Chính Sách" -> "chinh-sach"
            var slug = !string.IsNullOrWhiteSpace(request.Slug)
                ? GenerateSlug(request.Slug)
                : GenerateSlug(request.Title);

            // Check slug tồn tại trong DB chưa (Optional - Cần thêm hàm check trong Repo)

            var page = new Page
            {
                Title = request.Title,
                Slug = slug,
                Description = request.Description,
                ContentJson = "",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Page>().AddAsync(page);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(page.Slug);
        }

        // Hàm helper đơn giản tạo slug
        private string GenerateSlug(string phrase)
        {
            // Logic đơn giản: Chuyển thường, thay khoảng trắng bằng gạch ngang
            // Bạn có thể dùng thư viện Slugify để xịn hơn
            return phrase.ToLower().Replace(" ", "-");
        }
    }
}
