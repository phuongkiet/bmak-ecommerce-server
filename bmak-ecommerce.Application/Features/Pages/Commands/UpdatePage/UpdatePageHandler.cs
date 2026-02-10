using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Page;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Commands.UpdatePage
{
    [AutoRegister]

    public class UpdatePageHandler : ICommandHandler<UpdatePageCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy Page từ DB
            var page = await _unitOfWork.Repository<Page>().GetByIdAsync(request.Id);
            if (page == null) return Result<string>.Failure("Page not found");

            // 2. Update các trường cơ bản
            page.Title = request.Title;
            page.Status = request.IsPublished;
            page.Description = request.Description;
            page.UpdatedAt = DateTime.UtcNow;

            string inputSlug = !string.IsNullOrWhiteSpace(request.Slug) ? request.Slug : request.Title;

            // B2: Chuẩn hóa slug (hàm GenerateSlug ở dưới)
            string newSlug = GenerateSlug(inputSlug);

            // B3: Chỉ kiểm tra Database nếu Slug có thay đổi so với hiện tại
            if (page.Slug != newSlug)
            {
                // Kiểm tra xem có trang NÀO KHÁC (Id != request.Id) đang dùng slug này không
                // Lưu ý: Cần using Microsoft.EntityFrameworkCore để dùng FirstOrDefaultAsync
                var existingPage = await _unitOfWork.Pages.GetAllAsQueryable()
                    .FirstOrDefaultAsync(p => p.Slug == newSlug && p.Id != request.Id, cancellationToken);

                if (existingPage != null)
                {
                    return Result<string>.Failure($"Đường dẫn (Slug) '{newSlug}' đã tồn tại. Vui lòng chọn đường dẫn khác.");
                }

                // Nếu không trùng thì gán slug mới
                page.Slug = newSlug;
            }

            // 3. SERIALIZE: Chuyển List<PageSectionDto> -> JSON String
            // Dùng options để đảm bảo format đẹp và chuẩn camelCase
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false, // false để tiết kiệm dung lượng DB
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Lưu vào cột Content của Entity
            page.ContentJson = JsonSerializer.Serialize(request.Sections, jsonOptions);

            // 4. Lưu DB
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(page.Slug);
        }

        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;

            string str = phrase.ToLower().Trim();

            // Thay thế tiếng Việt có dấu
            str = Regex.Replace(str, "[áàảãạăắằẳẵặâấầẩẫậ]", "a");
            str = Regex.Replace(str, "[đ]", "d");
            str = Regex.Replace(str, "[éèẻẽẹêếềểễệ]", "e");
            str = Regex.Replace(str, "[íìỉĩị]", "i");
            str = Regex.Replace(str, "[óòỏõọôốồổỗộơớờởỡợ]", "o");
            str = Regex.Replace(str, "[úùủũụưứừửữự]", "u");
            str = Regex.Replace(str, "[ýỳỷỹỵ]", "y");

            // Thay thế ký tự đặc biệt bằng gạch ngang
            // \s+ : khoảng trắng
            // [^a-z0-9\-] : ký tự không phải chữ số hoặc gạch ngang
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-"); // Đổi khoảng trắng thành -
            str = Regex.Replace(str, @"-+", "-"); // Xóa gạch ngang lặp lại (vd: a--b -> a-b)

            return str.Trim('-');
        }
    }
}
