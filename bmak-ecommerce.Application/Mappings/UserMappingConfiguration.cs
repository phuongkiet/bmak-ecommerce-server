using AutoMapper;
using bmak_ecommerce.Application.Features.Users.Dtos;
using bmak_ecommerce.Domain.Entities.Identity;

namespace bmak_ecommerce.Application.Mappings
{
    public class UserMappingConfiguration : Profile
    {
        public UserMappingConfiguration()
        {
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.UserLevelName, opt => opt.MapFrom(src => src.UserLevel != null ? src.UserLevel.Name : null));
        }
    }
}
