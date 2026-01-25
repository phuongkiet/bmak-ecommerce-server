using bmak_ecommerce.Application.Features.Cart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Interfaces
{
    public interface ICartRepository
    {
        Task<ShoppingCart?> GetCartAsync(string cartId);
        Task<ShoppingCart?> UpdateCartAsync(ShoppingCart cart);
        Task<bool> DeleteCartAsync(string cartId);
    }
}
