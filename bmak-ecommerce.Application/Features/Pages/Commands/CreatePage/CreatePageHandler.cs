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

        public async Task<Result<string>> Handle(CreatePageCommand command, CancellationToken cancellationToken = default)
        {
            // 1. Kiểm tra Slug đã tồn tại chưa
            var slugExists = await _unitOfWork.Pages.GetPageDetailAsync(command.Slug);
            if (slugExists != null)
            {
                return Result<string>.Failure("Slug này đã tồn tại, vui lòng chọn slug khác.");
            }

            // 2. Khởi tạo Entity mới
            var newPage = new Page
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Slug = command.Slug.ToLower().Trim(),
                Description = command.Description,
                // Khởi tạo một mảng rỗng cho Sections để Frontend không bị lỗi khi load lần đầu
                ContentJson = JsonSerializer.Serialize(new List<object>()),
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _unitOfWork.Pages.Add(newPage);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Trả về Success kèm ID
                return Result<string>.Success(newPage.Slug);
            }
            catch (Exception ex)
            {
                // Log error here
                return Result<string>.Failure($"Lỗi khi tạo sản phẩm: {ex.Message}");
            }
        }
    }
}
