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
        public async Task<ActionResult<List<ProductAttributeListItemDto>>> GetProductAttributes()
        {
            var query = new GetProductAttributesQuery();
            var attributes = await _mediator.Send(query);
            return Ok(attributes);
        }

        // POST: api/productattributes
        [HttpPost]
        public async Task<ActionResult<int>> CreateProductAttribute([FromBody] CreateProductAttributeCommand command)
        {
            var attributeId = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateProductAttribute), new { id = attributeId }, attributeId);
        }

        // POST: api/productattributes/values
        [HttpPost("values")]
        public async Task<ActionResult<int>> CreateProductAttributeValue([FromBody] CreateProductAttributeValueCommand command)
        {
            var valueId = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateProductAttributeValue), new { id = valueId }, valueId);
        }
    }
}

