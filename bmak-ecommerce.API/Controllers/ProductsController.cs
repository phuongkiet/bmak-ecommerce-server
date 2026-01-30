using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models; // Import Result
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using bmak_ecommerce.Application.Features.Products.Commands.UpdateProduct;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetTopSellingProduct;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    // Kế thừa BaseApiController để dùng hàm HandleResult
    public class ProductsController : BaseApiController
    {
        // Lưu ý: Update lại kiểu trả về trong Interface là Result<T>
        private readonly IQueryHandler<GetProductsQuery, ProductListResponse> _getProductsHandler;
        private readonly IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>> _topSellingHandler;
        private readonly IQueryHandler<GetProductByIdQuery, ProductDto?> _getProductByIdHandler;

        // Command trả về Result<int>
        private readonly ICommandHandler<CreateProductCommand, int> _createProductHandler;

        // Command Update trả về Result<int> (hoặc Result nếu không cần trả ID)
        private readonly ICommandHandler<UpdateProductCommand, int> _updateProductHandler;

        public ProductsController(
            IQueryHandler<GetProductsQuery, ProductListResponse> getProductsHandler,
            IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>> topSellingHandler,
            IQueryHandler<GetProductByIdQuery, ProductDto?> getProductByIdHandler,
            ICommandHandler<CreateProductCommand, int> createProductHandler,
            ICommandHandler<UpdateProductCommand, int> updateProductHandler)
        {
            _getProductsHandler = getProductsHandler;
            _topSellingHandler = topSellingHandler;
            _getProductByIdHandler = getProductByIdHandler;
            _createProductHandler = createProductHandler;
            _updateProductHandler = updateProductHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ProductListResponse>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var query = new GetProductsQuery(specParams);

            // Result Pattern
            var result = await _getProductsHandler.Handle(query);

            // Xử lý Pagination Header (chỉ thêm nếu thành công)
            if (result.IsSuccess && result.Value?.Products != null)
            {
                Response.AddPaginationHeader(
                    result.Value.Products.PageIndex,
                    result.Value.Products.PageSize,
                    result.Value.Products.TotalCount,
                    result.Value.Products.TotalPages
                );
            }

            // Dùng hàm của BaseController
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);

            // Gọi Handler -> nhận Result<ProductDto?>
            var result = await _getProductByIdHandler.Handle(query, CancellationToken.None);

            // Trả về HTTP 200 (Success) hoặc 404 (Failure)
            return HandleResult(result);
        }

        [HttpGet("top-selling")]
        public async Task<ActionResult<List<ProductSummaryDto>>> GetTopSelling([FromQuery] int count = 10)
        {
            var query = new GetTopSellingProductsQuery { Count = count };

            // Gọi Handler -> nhận Result<List<ProductSummaryDto>>
            var result = await _topSellingHandler.Handle(query, CancellationToken.None);

            // BaseApiController xử lý trả về
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _createProductHandler.Handle(command);

            // Custom trả về 201 Created thay vì 200 OK mặc định của HandleResult
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
            }

            return BadRequest(new { message = result.Error });
        }

        [HttpPut("{id}")]
            public async Task<ActionResult<int>> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                command.Id = id;
            }

            // result là Result<int>
            var result = await _updateProductHandler.Handle(command, CancellationToken.None);

            // HandleResult trả về ActionResult<int> -> Khớp với khai báo hàm -> Hết lỗi
            return HandleResult(result);
        }
    }
}