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
        private readonly IQueryHandler<GetProductsQuery, PagedList<ProductDto>> _getProductsHandler;

        public ProductsController(IMediator mediator, IQueryHandler<GetProductsQuery, PagedList<ProductDto>> getProductsHandler)
        {
            _mediator = mediator;
            _getProductsHandler = getProductsHandler;
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
        public async Task<ActionResult<PagedList<ProductDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            // 1. Tạo Query Object
            var query = new GetProductsQuery(specParams);

            // 2. Gọi Handler xử lý trực tiếp
            var result = await _getProductsHandler.Handle(query);

            // 3. Trả về kết quả
            Response.AddPaginationHeader(result.PageIndex, result.PageSize, result.TotalCount, result.TotalPages);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null) return NotFound();

            return Ok(result);
        }

        // API lấy Top 10 bán chạy
        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSelling()
        {
            // Bây giờ nó sẽ hiểu GetTopSellingProductsQuery là gì
            var query = new GetTopSellingProductsQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<int>> CreateProduct([FromBody] CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProducts), new { id = productId }, productId);
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
