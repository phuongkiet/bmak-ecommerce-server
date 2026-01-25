using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Cart.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDatabase _database;

        public CartRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<ShoppingCart?> GetCartAsync(string cartId)
        {
            var data = await _database.StringGetAsync(cartId);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(data);
        }

        public async Task<ShoppingCart?> UpdateCartAsync(ShoppingCart cart)
        {
            var json = JsonSerializer.Serialize(cart);

            var created = await _database.StringSetAsync(cart.Id, json, TimeSpan.FromDays(30));

            if (!created) return null;

            return cart;
        }

        public async Task<bool> DeleteCartAsync(string cartId)
        {
            return await _database.KeyDeleteAsync(cartId);
        }
    }
}
