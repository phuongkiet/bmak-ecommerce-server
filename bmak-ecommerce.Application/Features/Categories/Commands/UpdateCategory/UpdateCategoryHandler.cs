using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Categories.Commands.UpdateCategory
{
    [AutoRegister]

    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy category hiện tại từ DB
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Category với ID: {request.Id}");
            }

            // 2. Validate không được set ParentId = chính nó (tránh circular reference)
            if (request.ParentId.HasValue && request.ParentId.Value == request.Id)
            {
                throw new InvalidOperationException("Không thể set danh mục cha là chính nó");
            }

            // 3. Normalize ParentId
            if (request.ParentId.HasValue && request.ParentId.Value <= 0)
            {
                category.ParentId = null;
            }
            else if (request.ParentId.HasValue && request.ParentId.Value > 0)
            {
                // Validate ParentId tồn tại
                var parentExists = await _unitOfWork.Repository<Category>()
                    .GetByIdAsync(request.ParentId.Value);
                
                if (parentExists == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy danh mục cha với ID {request.ParentId.Value}");
                }

                category.ParentId = request.ParentId.Value;
            }
            else
            {
                category.ParentId = null;
            }

            // 4. Update các fields
            category.Name = request.Name;
            category.Description = request.Description;

            // Update Slug nếu Name thay đổi
            if (category.Name != request.Name)
            {
                category.Slug = GenerateSlug(request.Name);
            }

            // 5. Update entity
            _unitOfWork.Repository<Category>().Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        private string GenerateSlug(string phrase)
        {
            return phrase.ToLower()
                .Replace(" ", "-")
                .Replace("đ", "d")
                .Replace("Đ", "d");
        }
    }
}


