using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.RoomScenes.DTOs;
using bmak_ecommerce.Domain.Entities.Visualizer;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.RoomScenes.Queries.GetRoomSceneDetail
{
    [AutoRegister]
    public class GetRoomSceneDetailHandler : IQueryHandler<GetRoomSceneDetailQuery, RoomSceneDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomSceneDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RoomSceneDto>> Handle(GetRoomSceneDetailQuery query, CancellationToken cancellationToken = default)
        {
            var repository = _unitOfWork.Repository<RoomScene>();
            var roomSceneQuery = repository.GetAllAsQueryable()
                .Where(x => !x.IsDeleted && x.Id == query.Id);

            if (!query.IncludeInactive)
            {
                roomSceneQuery = roomSceneQuery.Where(x => x.IsActive);
            }

            var roomScene = await roomSceneQuery
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
                .FirstOrDefaultAsync(cancellationToken);

            if (roomScene == null)
            {
                return Result<RoomSceneDto>.Failure("Khong tim thay phong mau.");
            }

            return Result<RoomSceneDto>.Success(roomScene);
        }
    }
}
