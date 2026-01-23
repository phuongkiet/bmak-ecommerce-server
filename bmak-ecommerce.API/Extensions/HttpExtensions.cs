using System.Text.Json;

namespace bmak_ecommerce.API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new
            {
                currentPage,
                itemsPerPage,
                totalItems,
                totalPages
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Thêm header "Pagination" chứa JSON
            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, options));

            // Quan trọng: Phải Expose header này ra thì Client (Angular/React) mới đọc được
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
