using CodeBattleArena.Domain.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is null ? Ok() : Ok(result.Value);
        }

        return result.Error.StatusCode switch
        {
            400 => BadRequest(CreateErrorResponse(result)),
            401 => Unauthorized(CreateErrorResponse(result)),
            403 => Forbid(),
            404 => NotFound(CreateErrorResponse(result)),
            _ => StatusCode(500, CreateErrorResponse(result))
        };
    }

    private object CreateErrorResponse<T>(Result<T> result) => new
    {
        code = result.Error.Code,
        message = result.Error.Message,
        errors = result.Errors.Any() ? result.Errors : null
    };
}