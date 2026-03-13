using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Users.Dtos;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Users.Queries.GetUserLevels
{
    [AutoRegister]
    public class GetUserLevelsHandler : IQueryHandler<GetUserLevelsQuery, List<UserLevelDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserLevelsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<UserLevelDto>>> Handle(GetUserLevelsQuery request, CancellationToken cancellationToken = default)
        {
            var levels = await _unitOfWork.Repository<UserLevel>()
                .GetAllAsQueryable()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Rank)
                .Select(x => new UserLevelDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Rank = x.Rank,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Result<List<UserLevelDto>>.Success(levels);
        }
    }
}
