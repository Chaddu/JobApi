using Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApi.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result),
            ErrorType.Unauthorized => Unauthorized(result),
            ErrorType.Forbidden => StatusCode(403, result),
            ErrorType.Conflict => Conflict(result),
            ErrorType.Validation => BadRequest(result),
            _ => StatusCode(500, result)
        };
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok(result);

        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result),
            ErrorType.Unauthorized => Unauthorized(result),
            ErrorType.Forbidden => StatusCode(403, result),
            ErrorType.Conflict => Conflict(result),
            ErrorType.Validation => BadRequest(result),
            _ => StatusCode(500, result)
        };
    }

    protected int GetUserId()
    {
        return int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}