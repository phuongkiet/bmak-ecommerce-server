using AutoMapper;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Tags.Commands.CreateTag
{
    public class CreateTagHandler : IRequestHandler<CreateTagCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTagHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            // 1. Map Command -> Entity
            var tag = _mapper.Map<Tag>(request);

            // 2. Logic nghiệp vụ bổ sung
            // Tự động sinh Slug từ Name (Ví dụ: "Bán chạy" -> "ban-chay")
            tag.Slug = GenerateSlug(tag.Name);

            // 3. Validate Slug phải unique
            var existingTags = await _unitOfWork.Repository<Tag>()
                .FindAsync(t => t.Slug == tag.Slug);
            
            if (existingTags.Any())
            {
                throw new InvalidOperationException($"Đã tồn tại tag với slug: {tag.Slug}");
            }

            // 4. Lưu vào DB thông qua UnitOfWork
            await _unitOfWork.Repository<Tag>().AddAsync(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Trả về ID
            return tag.Id;
        }

        // Helper sinh slug đơn giản (giống Category và Product)
        private string GenerateSlug(string phrase)
        {
            // Logic đơn giản: chuyển thường, thay khoảng trắng bằng gạch ngang
            // Thực tế nên dùng thư viện "Slugify" để xử lý tiếng Việt có dấu
            return phrase.ToLower()
                .Replace(" ", "-")
                .Replace("đ", "d")
                .Replace("Đ", "d");
        }
    }
}


