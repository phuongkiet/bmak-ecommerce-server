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

namespace bmak_ecommerce.Application.Features.Tags.Commands.CreateTag
{
    [AutoRegister]
    // 1. Đổi interface thành ICommandHandler
    public class CreateTagHandler : ICommandHandler<CreateTagCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTagHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 2. Đổi kiểu trả về thành Task<Result<int>>
        public async Task<Result<int>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            // Map Command -> Entity
            var tag = _mapper.Map<Tag>(request);

            // Logic sinh Slug
            tag.Slug = GenerateSlug(tag.Name);

            // Validate Slug
            var existingTags = await _unitOfWork.Repository<Tag>()
                .FindAsync(t => t.Slug == tag.Slug);

            if (existingTags.Any())
            {
                // 3. Trả về Result.Failure thay vì throw Exception (Clean hơn)
                return Result<int>.Failure($"Đã tồn tại tag với slug: {tag.Slug}");
            }

            try
            {
                // Lưu vào DB
                await _unitOfWork.Repository<Tag>().AddAsync(tag);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 4. Trả về Result.Success
                return Result<int>.Success(tag.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Lỗi khi tạo tag: {ex.Message}");
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


