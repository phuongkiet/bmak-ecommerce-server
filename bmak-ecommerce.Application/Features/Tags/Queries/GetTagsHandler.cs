using AutoMapper;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Tags.Queries
{
    public class GetTagsHandler : IRequestHandler<GetTagsQuery, List<TagDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTagsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy tất cả tags từ repository
            var tags = await _unitOfWork.Repository<Tag>().GetAllAsync();

            // 2. Map Entity -> DTO
            var tagDtos = _mapper.Map<List<TagDto>>(tags);

            // 3. Trả về danh sách tags
            return tagDtos;
        }
    }
}


