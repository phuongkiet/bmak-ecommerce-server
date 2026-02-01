using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Pages.DTOs;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Domain.Entities.Page;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.Queries.GetPageDetail
{
    public class GetPageDetailHandler : IQueryHandler<GetPageDetailQuery, PageDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPageDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PageDto?>> Handle(GetPageDetailQuery request, CancellationToken cancellationToken)
        {
            // 1. Gọi hàm Repository chuyên dụng đã viết ở Bước 1
            var page = await _unitOfWork.Pages.GetPageDetailAsync(request.Slug);

            // 2. Kiểm tra Null -> Trả về Failure
            if (page == null)
            {
                return Result<PageDto?>.Failure($"Không tìm thấy trang có Slug = {request.Slug}");
            }

            // 3. Map Entity -> DTO
            var pageDto = _mapper.Map<PageDto>(page);

            // 5. Trả về Success
            return Result<PageDto?>.Success(pageDto);
        }
    }
}
