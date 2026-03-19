using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Visualizer;
using bmak_ecommerce.Domain.Interfaces;

namespace bmak_ecommerce.Application.Features.RoomScenes.Commands.DeleteRoomScene
{
    [AutoRegister]
    public class DeleteRoomSceneHandler : ICommandHandler<DeleteRoomSceneCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomSceneHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteRoomSceneCommand command, CancellationToken cancellationToken = default)
        {
            var repository = _unitOfWork.Repository<RoomScene>();
            var roomScene = await repository.GetByIdAsync(command.Id);
            if (roomScene == null || roomScene.IsDeleted)
            {
                return Result<bool>.Failure("Khong tim thay phong mau.");
            }

            roomScene.IsDeleted = true;
            roomScene.UpdatedAt = DateTime.UtcNow;

            repository.Update(roomScene);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
