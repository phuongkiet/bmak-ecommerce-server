using AutoMapper;
using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
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
    [AutoRegister]
    public class GetTagsHandler : IQueryHandler<GetTagsQuery, List<TagDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTagsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<TagDto>>> Handle(GetTagsQuery query, CancellationToken cancellationToken)
        {
            var tags = await _unitOfWork.Repository<Tag>().GetAllAsync();


            var tagDtos = _mapper.Map<List<TagDto>>(tags);

            return Result<List<TagDto>>.Success(tagDtos);
        }
    }
}


