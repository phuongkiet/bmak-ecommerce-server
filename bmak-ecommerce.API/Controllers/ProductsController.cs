using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Features.Products.Commands.CreateProduct;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetAllProducts;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById;
using bmak_ecommerce.Application.Features.Products.Queries.Products.GetTopSellingProduct;
using bmak_ecommerce.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IQueryHandler<GetProductsQuery, ProductListResponse> _getProductsHandler;
        private readonly IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>> _topSellingHandler;
        private readonly IQueryHandler<GetProductByIdQuery, ProductDto?> _getProductByIdHandler;
        private readonly ICommandHandler<CreateProductCommand, int> _createProductHandler;

        public ProductsController(IMediator mediator, 
            IQueryHandler<GetProductsQuery, ProductListResponse> getProductsHandler,
            IQueryHandler<GetTopSellingProductsQuery, List<ProductSummaryDto>> topSellingHandler,
            IQueryHandler<GetProductByIdQuery, ProductDto?> getProductByIdHandler,
            ICommandHandler<CreateProductCommand, int> createProductHandler)
        {
            _mediator = mediator;
            _getProductsHandler = getProductsHandler;
            _topSellingHandler = topSellingHandler;
            _getProductByIdHandler = getProductByIdHandler;
            _createProductHandler = createProductHandler;
        }

        //// GET: api/products?pageIndex=1&pageSize=10&sort=priceAsc&attributes=size:60x60
        //[HttpGet]
        //public async Task<ActionResult<PagedList<ProductDto>>> GetProducts([FromQuery] ProductSpecParams productParams)
        //{
        //    var query = new GetProductsQuery(productParams);
        //    var result = await _mediator.Send(query);
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult<ProductListResponse>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var query = new GetProductsQuery(specParams);

            // Gọi Handler
            var result = await _getProductsHandler.Handle(query);

            if (result?.Products != null)
            {
                Response.AddPaginationHeader(
                    result.Products.PageIndex,
                    result.Products.PageSize,
                    result.Products.TotalCount,
                    result.Products.TotalPages
                );
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);

            var result = await _getProductByIdHandler.Handle(query, CancellationToken.None);

            if (result == null) return NotFound();

            return Ok(result);
        }

        // API lấy Top 10 bán chạy
        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSelling()
        {
            var query = new GetTopSellingProductsQuery { Count = 10 };

            // Gọi hàm Handle trực tiếp
            var result = await _topSellingHandler.Handle(query, CancellationToken.None);

            return Ok(result);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<int>> CreateProduct([FromBody] CreateProductCommand command)
        {
            // Gọi trực tiếp Handle, không qua MediatR
            var productId = await _createProductHandler.Handle(command, CancellationToken.None);

            // Trả về 201 Created
            return CreatedAtAction(nameof(GetById), new { id = productId }, productId);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateProduct(int id, [FromBody] bmak_ecommerce.Application.Features.Products.Commands.UpdateProduct.UpdateProductCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
