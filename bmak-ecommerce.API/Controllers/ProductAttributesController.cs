using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.ProductAttributes.Commands.CreateProductAttribute;
using bmak_ecommerce.Application.Features.ProductAttributes.Queries;
using bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.CreateProductAttributeValue;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAttributesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductAttributesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/productattributes
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductAttributeListItemDto>>>> GetProductAttributes()
        {
            try
            {
                var query = new GetProductAttributesQuery();
                var attributes = await _mediator.Send(query);
                return Ok(ApiResponse<List<ProductAttributeListItemDto>>.Success(attributes, "Product attributes retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ProductAttributeListItemDto>>.Failure(ex.Message));
            }
        }

        // POST: api/productattributes
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateProductAttribute([FromBody] CreateProductAttributeCommand command)
        {
            try
            {
                var attributeId = await _mediator.Send(command);
                return CreatedAtAction(nameof(CreateProductAttribute), new { id = attributeId }, ApiResponse<int>.Success(attributeId, "Product attribute created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<int>.Failure(ex.Message));
            }
        }

        // POST: api/productattributes/values
        [HttpPost("values")]
        public async Task<ActionResult<ApiResponse<int>>> CreateProductAttributeValue([FromBody] CreateProductAttributeValueCommand command)
        {
            try
            {
                var valueId = await _mediator.Send(command);
                return CreatedAtAction(nameof(CreateProductAttributeValue), new { id = valueId }, ApiResponse<int>.Success(valueId, "Product attribute value created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<int>.Failure(ex.Message));
            }
        }
    }
}

