using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Cart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Cart.Queries.GetCart
{
    public class GetCartHandler : IQueryHandler<GetCartQuery, ShoppingCart>
    {
        private readonly ICartRepository _cartRepository;

        public GetCartHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ShoppingCart> Handle(GetCartQuery query, CancellationToken cancellationToken)
        {
            // Nếu không tìm thấy, trả về giỏ rỗng để FE không bị lỗi null
            return await _cartRepository.GetCartAsync(query.CartId)
                   ?? new ShoppingCart(query.CartId);
        }
    }
}
