using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.RoomScenes.Commands.Common;
using bmak_ecommerce.Domain.Entities.Visualizer;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.RoomScenes.Commands.UpdateRoomScene
{
    [AutoRegister]
    public class UpdateRoomSceneHandler : ICommandHandler<UpdateRoomSceneCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoomSceneHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateRoomSceneCommand command, CancellationToken cancellationToken = default)
        {
            var repository = _unitOfWork.Repository<RoomScene>();
            var roomScene = await repository.GetByIdAsync(command.Id);
            if (roomScene == null || roomScene.IsDeleted)
            {
                return Result<bool>.Failure("Khong tim thay phong mau.");
            }

            var slug = RoomSceneSlugHelper.GenerateSlug(command.Title);
            if (string.IsNullOrWhiteSpace(slug))
            {
                return Result<bool>.Failure("Khong tao duoc slug tu ten phong mau.");
            }

            var isSlugExisted = await repository.GetAllAsQueryable()
                .AnyAsync(x => !x.IsDeleted && x.Slug == slug && x.Id != command.Id, cancellationToken);
            if (isSlugExisted)
            {
                return Result<bool>.Failure("Phong mau voi slug nay da ton tai.");
            }

            roomScene.Title = command.Title.Trim();
            roomScene.Slug = slug;
            roomScene.ConfigJson = command.ConfigJson.Trim();
            roomScene.RoomLayerUrl = command.RoomLayerUrl.Trim();
            roomScene.MattLayerUrl = command.MattLayerUrl.Trim();
            roomScene.GlossyLayerUrl = command.GlossyLayerUrl.Trim();
            roomScene.ThumbnailUrl = command.ThumbnailUrl.Trim();
            roomScene.IsActive = command.IsActive;
            roomScene.UpdatedAt = DateTime.UtcNow;

            repository.Update(roomScene);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
