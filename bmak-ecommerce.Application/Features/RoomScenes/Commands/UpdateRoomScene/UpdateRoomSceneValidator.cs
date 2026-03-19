using FluentValidation;

namespace bmak_ecommerce.Application.Features.RoomScenes.Commands.UpdateRoomScene
{
    public class UpdateRoomSceneValidator : AbstractValidator<UpdateRoomSceneCommand>
    {
        public UpdateRoomSceneValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id phong mau khong hop le.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Ten phong mau la bat buoc.")
                .MaximumLength(255).WithMessage("Ten phong mau toi da 255 ky tu.");

            RuleFor(x => x.ConfigJson)
                .NotEmpty().WithMessage("Cau hinh JSON la bat buoc.");

            RuleFor(x => x.RoomLayerUrl)
                .NotEmpty().WithMessage("Layer Room la bat buoc.")
                .MaximumLength(1000).WithMessage("Layer Room toi da 1000 ky tu.");

            RuleFor(x => x.MattLayerUrl)
                .NotEmpty().WithMessage("Layer Matt la bat buoc.")
                .MaximumLength(1000).WithMessage("Layer Matt toi da 1000 ky tu.");

            RuleFor(x => x.GlossyLayerUrl)
                .NotEmpty().WithMessage("Layer Glossy la bat buoc.")
                .MaximumLength(1000).WithMessage("Layer Glossy toi da 1000 ky tu.");

            RuleFor(x => x.ThumbnailUrl)
                .NotEmpty().WithMessage("Thumbnail la bat buoc.")
                .MaximumLength(1000).WithMessage("Thumbnail toi da 1000 ky tu.");
        }
    }
}
