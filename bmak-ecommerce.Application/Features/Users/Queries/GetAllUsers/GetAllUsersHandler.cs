using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Users.Dtos;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Queries.GetAllUsers
{
    [AutoRegister]

    public class GetAllUsersHandler : IQueryHandler<GetAllUsersQuery, PagedList<UserSummaryDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public GetAllUsersHandler(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<UserSummaryDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            // Query trực tiếp từ UserManager (là IQueryable)
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Params.Search))
            {
                var search = request.Params.Search.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(search) || u.FullName.ToLower().Contains(search));
            }

            // Filter Soft Delete (Mặc định không hiện user đã xóa, trừ khi admin yêu cầu)
            // query = query.Where(u => !u.IsDeleted); 

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .Skip((request.Params.PageIndex - 1) * request.Params.PageSize)
                .Take(request.Params.PageSize)
                .ToListAsync(cancellationToken);

            // Map thủ công hoặc AutoMapper để lấy Role string
            // Lưu ý: Lấy Role trong loop có thể gây N+1 query.
            // Cách tối ưu: Join bảng UserRoles nhưng UserManager không support trực tiếp dễ dàng.
            // Ở đây demo cách đơn giản:
            var dtos = new List<UserSummaryDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                dtos.Add(new UserSummaryDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsDeleted = user.IsDeleted,
                    Roles = string.Join(", ", roles)
                });
            }

            var pagedList = new PagedList<UserSummaryDto>(dtos, totalCount, request.Params.PageIndex, request.Params.PageSize);
            return Result<PagedList<UserSummaryDto>>.Success(pagedList);
        }
    }
}
