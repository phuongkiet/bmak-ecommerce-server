using AutoMapper;
using bmak_ecommerce.Application.Features.Pages.DTOs;
using bmak_ecommerce.Domain.Entities.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Mappings
{
    public class PageMappingConfiguration : Profile
    {
        public PageMappingConfiguration()
        {
            CreateMap<Page, PageDto>()
                .ForMember(dest => dest.Sections, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<Page, PageSummaryDto>().ReverseMap();
        }
    }
}
