using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        // Hàm helper xử lý Result<T> và trả về ApiResponse<T>
        protected ActionResult<ApiResponse<T>> HandleResult<T>(Result<T> result)
        {
            if (result == null) 
                return NotFound(ApiResponse<T>.Failure("Resource not found"));

            if (result.IsSuccess && result.Value != null)
                return Ok(ApiResponse<T>.Success(result.Value, "Success"));

            if (result.IsSuccess && result.Value == null)
                return NotFound(ApiResponse<T>.Failure("Resource not found"));

            return BadRequest(ApiResponse<T>.Failure(result.Error ?? "An error occurred"));
        }

        // Hàm helper xử lý Result (không có data, vd: Update/Delete)
        protected ActionResult<ApiResponse> HandleResult(Result result)
        {
            if (result == null) 
                return NotFound(ApiResponse.Failure("Resource not found"));

            if (result.IsSuccess)
                return Ok(ApiResponse.Success("Operation completed successfully"));

            return BadRequest(ApiResponse.Failure(result.Error ?? "An error occurred"));
        }
    }
}