using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Helpers;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Addresses.DTOs;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Addresses.Queries.GetAddressDetail
{
    [AutoRegister]
    public class GetAddressDetailHandler : IQueryHandler<GetAddressDetailQuery, AddressDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetAddressDetailHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AddressDto>> Handle(GetAddressDetailQuery query, CancellationToken cancellationToken = default)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<AddressDto>.Failure("Bạn cần đăng nhập để xem địa chỉ.");
            }

            var address = await _unitOfWork.Repository<Address>()
                .GetAllAsQueryable()
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.ReceiverName,
                    x.Phone,
                    x.Street,
                    x.ProvinceId,
                    ProvinceName = x.Province.Name,
                    x.WardId,
                    WardName = x.Ward.Name,
                    x.Type,
                    x.UserId,
                    x.CreatedAt,
                    x.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (address == null)
            {
                return Result<AddressDto>.Failure("Không tìm thấy địa chỉ.");
            }

            if (address.UserId != _currentUserService.UserId)
            {
                return Result<AddressDto>.Failure("Bạn không có quyền xem địa chỉ này.");
            }

            return Result<AddressDto>.Success(new AddressDto
            {
                Id = address.Id,
                ReceiverName = address.ReceiverName,
                Phone = address.Phone,
                Street = address.Street,
                ProvinceId = address.ProvinceId,
                ProvinceName = address.ProvinceName,
                Zone = ShippingZoneHelper.Resolve(address.ProvinceId, address.ProvinceName),
                WardId = address.WardId,
                WardName = address.WardName,
                Type = address.Type,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            });
        }
    }
}
