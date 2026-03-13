using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Users.Dtos;
using bmak_ecommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Users.Queries.GetUserDetail
{
    [AutoRegister]
    public class GetUserDetailHandler : IQueryHandler<GetUserDetailQuery, UserDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public GetUserDetailHandler(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Include(x => x.UserLevel)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (user == null) return Result<UserDto>.Failure("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = roles.ToList();
            dto.UserLevelId = user.UserLevelId;
            dto.UserLevelName = user.UserLevel?.Name;

            return Result<UserDto>.Success(dto);
        }
    }
}
