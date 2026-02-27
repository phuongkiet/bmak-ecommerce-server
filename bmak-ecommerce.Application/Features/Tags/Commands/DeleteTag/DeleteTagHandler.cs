using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Media;
using bmak_ecommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace bmak_ecommerce.Application.Features.Tags.Commands.DeleteTag
{
    [AutoRegister]
    public class DeleteTagHandler : ICommandHandler<DeleteTagCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteTagHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteTagCommand command, CancellationToken cancellationToken = default)
        {
            var tagRepo = _unitOfWork.Repository<Tag>();
            var tag = await tagRepo.GetByIdAsync(command.Id);

            if (tag == null)
                return Result<bool>.Failure("Thẻ không tồn tại trong hệ thống.");

            tagRepo.Remove(tag);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
