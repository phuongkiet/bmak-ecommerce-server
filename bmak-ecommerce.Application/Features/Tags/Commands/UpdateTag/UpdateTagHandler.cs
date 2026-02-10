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

namespace bmak_ecommerce.Application.Features.Tags.Commands.UpdateTag
{
    [AutoRegister]

    public class UpdateTagHandler : IRequestHandler<UpdateTagCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTagHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy tag hiện tại từ DB
            var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(request.Id);
            if (tag == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Tag với ID: {request.Id}");
            }

            // 2. Update các fields
            tag.Name = request.Name;
            tag.Description = request.Description;

            // 3. Update Slug nếu Name thay đổi và validate unique
            if (tag.Name != request.Name)
            {
                var newSlug = GenerateSlug(request.Name);
                
                // Validate Slug phải unique (trừ chính nó)
                var existingTags = await _unitOfWork.Repository<Tag>()
                    .FindAsync(t => t.Slug == newSlug);
                
                if (existingTags.Any(t => t.Id != request.Id))
                {
                    throw new InvalidOperationException($"Đã tồn tại tag với slug: {newSlug}");
                }

                tag.Slug = newSlug;
            }

            // 4. Update entity
            _unitOfWork.Repository<Tag>().Update(tag);
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


