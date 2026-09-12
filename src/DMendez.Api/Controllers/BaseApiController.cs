using DMendez.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Api.Controllers
{
    /// <summary>
    /// Controlador base que estandariza la traducción del patrón Result a respuestas HTTP.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Value == null)
                    return NotFound(new { message = result.Error ?? "El recurso solicitado no fue encontrado." });

                return Ok(result.Value);
            }

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.Error }),
                ErrorType.Conflict => Conflict(new { message = result.Error }),
                ErrorType.Validation => BadRequest(new { message = result.Error, errors = result.Errors }),
                _ => BadRequest(new { message = result.Error })
            };
        }

        protected ActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return NoContent();

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { message = result.Error }),
                ErrorType.Conflict => Conflict(new { message = result.Error }),
                ErrorType.Validation => BadRequest(new { message = result.Error, errors = result.Errors }),
                _ => BadRequest(new { message = result.Error })
            };
        }

        protected ActionResult HandleCreatedResult<T>(Result<T> result, string actionName, object routeValues)
        {
            if (result.IsSuccess && result.Value != null)
                return CreatedAtAction(actionName, routeValues, result.Value);

            return HandleResult(result);
        }
    }
}
