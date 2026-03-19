using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.RoomScenes.Commands.Common;
using bmak_ecommerce.Domain.Entities.Visualizer;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.RoomScenes.Commands.CreateRoomScene
{
    [AutoRegister]
    public class CreateRoomSceneHandler : ICommandHandler<CreateRoomSceneCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoomSceneHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateRoomSceneCommand command, CancellationToken cancellationToken = default)
        {
            var repository = _unitOfWork.Repository<RoomScene>();
            var slug = RoomSceneSlugHelper.GenerateSlug(command.Title);

            if (string.IsNullOrWhiteSpace(slug))
            {
                return Result<int>.Failure("Khong tao duoc slug tu ten phong mau.");
            }

            var isSlugExisted = await repository.GetAllAsQueryable()
                .AnyAsync(x => !x.IsDeleted && x.Slug == slug, cancellationToken);
            if (isSlugExisted)
            {
                return Result<int>.Failure("Phong mau voi slug nay da ton tai.");
            }

            var roomScene = new RoomScene
            {
                Title = command.Title.Trim(),
                Slug = slug,
                ConfigJson = command.ConfigJson.Trim(),
                RoomLayerUrl = command.RoomLayerUrl.Trim(),
                MattLayerUrl = command.MattLayerUrl.Trim(),
                GlossyLayerUrl = command.GlossyLayerUrl.Trim(),
                ThumbnailUrl = command.ThumbnailUrl.Trim(),
                IsActive = command.IsActive
            };

            await repository.AddAsync(roomScene);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(roomScene.Id);
        }
    }
}
