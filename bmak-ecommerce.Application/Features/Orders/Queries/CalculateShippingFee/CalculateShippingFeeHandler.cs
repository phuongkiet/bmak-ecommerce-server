using bmak_ecommerce.Application.Common.Attributes;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.Orders.DTOs;
using bmak_ecommerce.Application.Features.Orders.Models;
using bmak_ecommerce.Domain.Interfaces;
using FluentValidation;

namespace bmak_ecommerce.Application.Features.Orders.Queries.CalculateShippingFee
{
    [AutoRegister]
    public class CalculateShippingFeeHandler : IQueryHandler<CalculateShippingFeeQuery, ShippingFeeResponseDto>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShippingRuleEngine _shippingRuleEngine;
        private readonly IValidator<CalculateShippingFeeQuery> _validator;

        public CalculateShippingFeeHandler(
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork,
            IShippingRuleEngine shippingRuleEngine,
            IValidator<CalculateShippingFeeQuery> validator)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _shippingRuleEngine = shippingRuleEngine;
            _validator = validator;
        }

        public async Task<Result<ShippingFeeResponseDto>> Handle(CalculateShippingFeeQuery request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<ShippingFeeResponseDto>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var cart = await _cartRepository.GetCartAsync(request.CartId);
            if (cart == null || !cart.Items.Any())
            {
                return Result<ShippingFeeResponseDto>.Failure("Giỏ hàng trống hoặc không tồn tại.");
            }

            var productIds = cart.Items.Select(x => x.ProductId).Distinct().ToList();
            var dbProducts = await _unitOfWork.Products.GetByIdsAsync(productIds);

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

                var unitPrice = product.SalePrice > 0 ? product.SalePrice : product.BasePrice;
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
                Ward = request.Ward
            };

            var shippingResult = await _shippingRuleEngine.CalculateAsync(context, cancellationToken);

            return Result<ShippingFeeResponseDto>.Success(new ShippingFeeResponseDto
            {
                ShippingFee = shippingResult.ShippingFee,
                SubTotal = subTotal,
                TotalWeight = totalWeight,
                TotalSquareMeter = totalSquareMeter,
                MatchedRules = shippingResult.MatchedRules
            });
        }
    }
}
