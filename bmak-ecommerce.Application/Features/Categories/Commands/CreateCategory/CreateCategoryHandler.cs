using AutoMapper;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // 1. Map Command -> Entity
            var category = _mapper.Map<Category>(request);

            // 2. Logic nghiệp vụ bổ sung
            // Tự động sinh Slug từ Name (Ví dụ: "Gạch ốp tường" -> "gach-op-tuong")
            category.Slug = GenerateSlug(category.Name);

            // Normalize ParentId: nếu là 0 hoặc null thì set thành null (danh mục gốc)
            if (request.ParentId.HasValue && request.ParentId.Value <= 0)
            {
                category.ParentId = null;
            }

            // Validate ParentId nếu có (chỉ validate khi > 0)
            if (category.ParentId.HasValue && category.ParentId.Value > 0)
            {
                var parentExists = await _unitOfWork.Repository<Category>()
                    .GetByIdAsync(category.ParentId.Value);
                
                if (parentExists == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy danh mục cha với ID {category.ParentId.Value}");
                }
            }

            // 3. Lưu vào DB thông qua UnitOfWork
            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. Trả về ID
            return category.Id;
        }

        // Helper sinh slug đơn giản (giống Product)
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

