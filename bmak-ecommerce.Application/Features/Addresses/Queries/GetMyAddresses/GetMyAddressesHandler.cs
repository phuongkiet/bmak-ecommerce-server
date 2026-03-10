using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Addresses.DTOs;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Addresses.Queries.GetMyAddresses
{
    [AutoRegister]
    public class GetMyAddressesHandler : IQueryHandler<GetMyAddressesQuery, List<AddressDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMyAddressesHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<AddressDto>>> Handle(GetMyAddressesQuery query, CancellationToken cancellationToken = default)
        {
            if (_currentUserService.UserId <= 0)
            {
                return Result<List<AddressDto>>.Failure("Bạn cần đăng nhập để xem danh sách địa chỉ.");
            }

            var addresses = await _unitOfWork.Repository<Address>()
                .GetAllAsQueryable()
                .Where(x => !x.IsDeleted && x.UserId == _currentUserService.UserId)
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .Select(x => new AddressDto
                {
                    Id = x.Id,
                    ReceiverName = x.ReceiverName,
                    Phone = x.Phone,
                    Street = x.Street,
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.Province.Name,
                    WardId = x.WardId,
                    WardName = x.Ward.Name,
                    Type = x.Type,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return Result<List<AddressDto>>.Success(addresses);
        }
    }
}
