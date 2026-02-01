using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Pages.DTOs;
using bmak_ecommerce.Domain.Interfaces;
using bmak_ecommerce.Domain.Models;

namespace bmak_ecommerce.Application.Features.Pages.Queries.GetAllPages
{
    public class GetPageHandler : IQueryHandler<GetPageQuery, PagedList<PageSummaryDto>>
    {
        private readonly IPageRepository _pageRepository;
        private readonly IMapper _mapper;

        public GetPageHandler(IPageRepository pageRepository, IMapper mapper)
        {
            _pageRepository = pageRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<PageSummaryDto>>> Handle(GetPageQuery query, CancellationToken cancellationToken = default)
        {
            // Gọi Repository xử lý logic phân trang và tìm kiếm đã viết
            var pagedPages = await _pageRepository.GetPagesAsync(query.Params);

            // Chuyển đổi sang Summary DTO để tối ưu dữ liệu truyền tải
            var dtos = _mapper.Map<IReadOnlyList<PageSummaryDto>>(pagedPages.Items).ToList();

            var result = new PagedList<PageSummaryDto>(
                dtos,
                pagedPages.TotalCount,
                pagedPages.PageIndex,
                pagedPages.PageSize
            );

            return Result<PagedList<PageSummaryDto>>.Success(result);
        }
    }
}
