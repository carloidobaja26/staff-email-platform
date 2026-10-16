using Microsoft.AspNetCore.Mvc;
using EmailPlatform.Application.Common.Responses;

namespace EmailPlatformApi.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(ApiResponse<T> response)
    {
        if (response.Success)
            return Ok(response);

        if (response.Errors?.Any() == true)
            return BadRequest(response);

        return NotFound(response);
    }
}