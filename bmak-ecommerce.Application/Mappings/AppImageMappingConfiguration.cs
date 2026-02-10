using AutoMapper;
using bmak_ecommerce.Application.Features.Media.DTOs;
using bmak_ecommerce.Domain.Entities.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Mappings
{
    public class AppImageMappingConfiguration : Profile
    {
        public AppImageMappingConfiguration()
        {
            CreateMap<AppImage, AppImageDto>().ReverseMap();
        }
    }
}
