using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Helpers;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Application.Features.Orders.Models;
using bmak_ecommerce.Domain.Entities.Catalog;
using bmak_ecommerce.Domain.Entities.Identity;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bmak_ecommerce.Application.Features.Orders.Queries.CalculateShippingFee
{
    [AutoRegister]
    public class CalculateShippingFeeHandler : IQueryHandler<CalculateShippingFeeQuery, ShippingFeeResponseDto>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShippingRuleEngine _shippingRuleEngine;
        private readonly IValidator<CalculateShippingFeeQuery> _validator;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public CalculateShippingFeeHandler(
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork,
            IShippingRuleEngine shippingRuleEngine,
            IValidator<CalculateShippingFeeQuery> validator,
            ICurrentUserService currentUserService,
            UserManager<AppUser> userManager)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _shippingRuleEngine = shippingRuleEngine;
            _validator = validator;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<ShippingFeeResponseDto>> Handle(CalculateShippingFeeQuery request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<ShippingFeeResponseDto>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var effectiveCartId = ResolveEffectiveCartId(request.CartId);
            var cart = await _cartRepository.GetCartAsync(effectiveCartId);
            if (cart == null || !cart.Items.Any())
            {
                return Result<ShippingFeeResponseDto>.Failure("Giỏ hàng trống hoặc không tồn tại.");
            }

            var productIds = cart.Items.Select(x => x.ProductId).Distinct().ToList();
            var dbProducts = await _unitOfWork.Products.GetByIdsAsync(productIds);

            int? userLevelId = null;
            var currentUserId = _currentUserService.UserId;
            if (currentUserId > 0)
            {
                userLevelId = await _userManager.Users
                    .Where(x => x.Id == currentUserId)
                    .Select(x => x.UserLevelId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var activeLevelDiscounts = new Dictionary<int, decimal>();
            if (userLevelId.HasValue)
            {
                var now = DateTime.UtcNow;
                activeLevelDiscounts = await _unitOfWork.Repository<ProductLevelDiscount>()
                    .GetAllAsQueryable()
                    .Where(x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.UserLevelId == userLevelId.Value &&
                        productIds.Contains(x.ProductId) &&
                        (!x.StartAt.HasValue || x.StartAt.Value <= now) &&
                        (!x.EndAt.HasValue || x.EndAt.Value >= now))
                    .ToDictionaryAsync(x => x.ProductId, x => x.DiscountPercent, cancellationToken);
            }

            decimal subTotal = 0;
            decimal totalWeight = 0;
            decimal totalSquareMeter = 0;
            int itemCount = 0;

            foreach (var item in cart.Items)
            {
                var product = dbProducts.FirstOrDefault(x => x.Id == item.ProductId);
                if (product == null)
                {
                    continue;
                }

                var unitPrice = CalculateFinalUnitPrice(product, activeLevelDiscounts);
                var conversionFactor = product.ConversionFactor > 0 ? (decimal)product.ConversionFactor : 1m;
                var productWeight = product.Weight > 0 ? (decimal)product.Weight : 0m;

                subTotal += unitPrice * item.Quantity;
                totalSquareMeter += conversionFactor * item.Quantity;
                totalWeight += productWeight * item.Quantity;
                itemCount += item.Quantity;
            }

            var context = new ShippingRuleContext
            {
                SubTotal = subTotal,
                TotalWeight = totalWeight,
                TotalSquareMeter = totalSquareMeter,
                ItemCount = itemCount,
                Province = request.Province,
                Ward = request.Ward,
                Zone = ShippingZoneHelper.Resolve(request.Province)
            };

            var shippingResult = await _shippingRuleEngine.CalculateAsync(context, cancellationToken);

            return Result<ShippingFeeResponseDto>.Success(new ShippingFeeResponseDto
            {
                ShippingFee = shippingResult.ShippingFee,
                SubTotal = subTotal,
                TotalWeight = totalWeight,
                TotalSquareMeter = totalSquareMeter,
                ResolvedZone = context.Zone,
                MatchedRules = shippingResult.MatchedRules
            });
        }

        private static decimal CalculateFinalUnitPrice(Product product, IReadOnlyDictionary<int, decimal> activeLevelDiscounts)
        {
            var basePrice = product.SalePrice > 0 ? product.SalePrice : product.BasePrice;

            if (!activeLevelDiscounts.TryGetValue(product.Id, out var percent) || percent <= 0)
                return basePrice;

            var finalPrice = basePrice * (1 - (percent / 100m));
            if (finalPrice < 0) finalPrice = 0;

            return Math.Round(finalPrice, 2, MidpointRounding.AwayFromZero);
        }

        private string ResolveEffectiveCartId(string? clientCartId)
        {
            if (_currentUserService.UserId > 0)
            {
                return $"cart:user:{_currentUserService.UserId}";
            }

            return clientCartId ?? string.Empty;
        }
    }
}
