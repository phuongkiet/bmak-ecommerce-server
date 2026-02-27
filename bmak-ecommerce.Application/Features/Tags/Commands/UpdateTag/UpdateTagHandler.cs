using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Tags.Commands.UpdateTag
{
    [AutoRegister]
    // 1. Đổi sang ICommandHandler
    public class UpdateTagHandler : ICommandHandler<UpdateTagCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTagHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 2. Trả về Task<Result<bool>>
        public async Task<Result<bool>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Lấy tag hiện tại từ DB
                var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(request.Id);
                if (tag == null)
                {
                    // Trả về Failure thay vì throw Exception
                    return Result<bool>.Failure($"Không tìm thấy Tag với ID: {request.Id}");
                }

                // 2. Update Slug nếu Name thay đổi và validate unique
                // Sửa lỗi logic cũ: Kiểm tra Slug mới thay vì Name vì Name đã có thể bị trim/lower
                var newSlug = GenerateSlug(request.Name);

                if (tag.Slug != newSlug)
                {
                    var existingTags = await _unitOfWork.Repository<Tag>()
                        .FindAsync(t => t.Slug == newSlug);

                    if (existingTags.Any(t => t.Id != request.Id))
                    {
                        return Result<bool>.Failure($"Đã tồn tại tag với slug: {newSlug}");
                    }

                    tag.Slug = newSlug;
                }

                // 3. Update các fields còn lại
                tag.Name = request.Name;
                tag.Description = request.Description;

                // 4. Update entity
                _unitOfWork.Repository<Tag>().Update(tag);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 5. Trả về Success
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Lỗi khi cập nhật tag: {ex.Message}");
            }
        }

        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;

            // 1. Đưa về chữ thường và xử lý chữ 'đ' (vì Unicode Normalize không xử lý được chữ đ)
            string str = phrase.ToLower().Trim();
            str = str.Replace("đ", "d");

            // 2. Tách dấu ra khỏi ký tự gốc (VD: 'á' -> 'a' + '´')
            var normalizedString = str.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                // Bỏ qua các ký tự là dấu (NonSpacingMark)
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // 3. Ghép lại thành chuỗi không dấu
            str = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // 4. Xóa các ký tự đặc biệt (chỉ giữ lại chữ cái, số, khoảng trắng và dấu gạch ngang)
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // 5. Thay khoảng trắng bằng dấu gạch ngang và xóa các dấu gạch ngang liên tiếp (VD: "a---b" -> "a-b")
            str = Regex.Replace(str, @"\s+", "-").Trim('-');
            str = Regex.Replace(str, @"-+", "-");

            return str;
        }
    }
}


