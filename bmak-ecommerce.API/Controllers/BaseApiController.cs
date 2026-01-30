using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace bmak_ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        // Hàm helper xử lý Result<T>
        protected ActionResult<T> HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();

            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);

            if (result.IsSuccess && result.Value == null)
                return NotFound();

            return BadRequest(new { message = result.Error });
        }

        // Hàm helper xử lý Result (không có data, vd: Update/Delete)
        protected ActionResult HandleResult(Result result)
        {
            if (result == null) return NotFound();

            if (result.IsSuccess)
                return NoContent(); // 204 cho Update/Delete thành công

            return BadRequest(new { message = result.Error });
        }
    }
}