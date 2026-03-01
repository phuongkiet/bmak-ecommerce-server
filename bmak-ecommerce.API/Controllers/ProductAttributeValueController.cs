using bmak_ecommerce.API.Extensions;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.DeleteProductAttributeValue;
using bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.UpdateProductAttributeValue;
using bmak_ecommerce.Application.Features.ProductAttributeValues.DTOs;
using bmak_ecommerce.Application.Features.ProductAttributeValues.Queries.GetAttributeValuesQuery;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAttributeValueController : BaseApiController
    {
        private readonly IQueryHandler<GetAttributeValuesQuery, List<ProductAttributeValueDto>> _getAttributeValueHandler;
        private readonly ICommandHandler<UpdateProductAttributeValueCommand, bool> _updateAttributeValueHandler;
        private readonly ICommandHandler<DeleteProductAttributeValueCommand, bool> _deleteAttributeValueHandler;

        public ProductAttributeValueController(
            IQueryHandler<GetAttributeValuesQuery, List<ProductAttributeValueDto>> getAttributeValueHandler,
            ICommandHandler<UpdateProductAttributeValueCommand, bool> updateAttributeValueHandler,
            ICommandHandler<DeleteProductAttributeValueCommand, bool> deleteAttributeValueHandler)
        {
            _getAttributeValueHandler = getAttributeValueHandler;
            _updateAttributeValueHandler = updateAttributeValueHandler;
            _deleteAttributeValueHandler = deleteAttributeValueHandler;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductAttributeValueDto>>>> GetAttributeValues([FromQuery] GetAttributeValuesQuery query)
        {
            // Handler trả về Result<List<ProductAttributeValueDto>>
            var result = await _getAttributeValueHandler.Handle(query, CancellationToken.None);

            // HandleResult sẽ đóng gói List này vào ApiResponse
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateAttributeValue(int id, [FromBody] UpdateProductAttributeValueCommand command)
        {
            command.Id = id;
            var result = await _updateAttributeValueHandler.Handle(command, CancellationToken.None);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAttributeValue(int id)
        {
            var command = new DeleteProductAttributeValueCommand { Id = id };
            var result = await _deleteAttributeValueHandler.Handle(command, CancellationToken.None);
            return HandleResult(result);
        }
    }
}
