using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Features.ProductAttributes.Commands.CreateProductAttribute;
using bmak_ecommerce.Application.Features.ProductAttributes.Commands.DeleteProductAttribute;
using bmak_ecommerce.Application.Features.ProductAttributes.Commands.UpdateProductAttribute;
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
        private readonly ICommandHandler<UpdateProductAttributeCommand, bool> _updateAttributeHandler;
        private readonly ICommandHandler<DeleteProductAttributeCommand, bool> _deleteAttributeHandler;

        public ProductAttributesController(IMediator mediator, 
            ICommandHandler<UpdateProductAttributeCommand, bool> updateAttributeHandler,
            ICommandHandler<DeleteProductAttributeCommand, bool> deleteAttributeHandler)
        {
            _mediator = mediator;
            _updateAttributeHandler = updateAttributeHandler;
            _deleteAttributeHandler = deleteAttributeHandler;
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

        /// <summary>
        /// Cập nhật Thuộc tính và danh sách Tùy chọn (Options)
        /// </summary>
        // PUT: api/Attributes/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateAttribute(int id, [FromBody] UpdateProductAttributeCommand command)
        {
            try
            {
                // Ép ID từ URL vào Command (Do model đã [JsonIgnore] ID)
                command.Id = id;

                // Gọi Handler (Pure CQRS)
                var result = await _updateAttributeHandler.Handle(command, default);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.Success(result.Value, "Cập nhật thuộc tính thành công"));
                }

                return BadRequest(ApiResponse<bool>.Failure(result.Error));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Failure($"Lỗi hệ thống: {ex.Message}"));
            }
        }
        /// <summary>
        /// Xóa một Thuộc tính (Sẽ bị chặn nếu thuộc tính đó đang có dữ liệu con)
        /// </summary>
        // DELETE: api/Attributes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAttribute(int id)
        {
            try
            {
                var command = new DeleteProductAttributeCommand { Id = id };

                // Giả sử bạn đã inject ICommandHandler<DeleteAttributeCommand, bool> _deleteAttributeHandler ở constructor
                var result = await _deleteAttributeHandler.Handle(command, default);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.Success(result.Value, "Xóa thuộc tính thành công"));
                }

                // Nếu bị kẹt ở bước check Value, nó sẽ bay vào đây và trả về HTTP 400 kèm câu thông báo lỗi
                return BadRequest(ApiResponse<bool>.Failure(result.Error));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Failure($"Lỗi hệ thống: {ex.Message}"));
            }
        }

    }
}

