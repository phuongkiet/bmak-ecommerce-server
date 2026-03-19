using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.RoomScenes.DTOs;
using bmak_ecommerce.Domain.Entities.Visualizer;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomScenes
{
    [AutoRegister]
    public class GetRoomScenesHandler : IQueryHandler<GetRoomScenesQuery, List<RoomSceneDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomScenesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<RoomSceneDto>>> Handle(GetRoomScenesQuery query, CancellationToken cancellationToken = default)
        {
            var repository = _unitOfWork.Repository<RoomScene>();
            var roomScenesQuery = repository.GetAllAsQueryable()
                .Where(x => !x.IsDeleted);

            if (!query.IncludeInactive)
            {
                roomScenesQuery = roomScenesQuery.Where(x => x.IsActive);
            }

            var data = await roomScenesQuery
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new RoomSceneDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    ConfigJson = x.ConfigJson,
                    RoomLayerUrl = x.RoomLayerUrl,
                    MattLayerUrl = x.MattLayerUrl,
                    GlossyLayerUrl = x.GlossyLayerUrl,
                    ThumbnailUrl = x.ThumbnailUrl,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Result<List<RoomSceneDto>>.Success(data);
        }
    }
}
