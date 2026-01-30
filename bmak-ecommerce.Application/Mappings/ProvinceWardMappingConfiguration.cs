using AutoMapper;
using bmak_ecommerce.Application.Features.Provinces.Dtos;
using bmak_ecommerce.Application.Features.Wards.Dtos;
using bmak_ecommerce.Domain.Entities.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Mappings
{
    public class ProvinceWardMappingConfiguration : Profile
    {
        public ProvinceWardMappingConfiguration() {
            CreateMap<Province, ProvinceDto>().ReverseMap();
            CreateMap<Ward, WardDto>()
                .ReverseMap();
        }
    }
}
