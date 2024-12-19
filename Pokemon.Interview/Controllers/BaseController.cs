using System;
using Microsoft.AspNetCore.Mvc;

namespace Pokemon.Interview.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    public async Task<IActionResult> ToActionResult<T>(Task<Result<T>> result)
    {
        var response = await result;

        switch (response.Status)
        {
            case Enums.ResultStatus.Ok:
                return Ok(response);

            case Enums.ResultStatus.NotFound:
                return NotFound(response);

            case Enums.ResultStatus.Invalid:
                return BadRequest(response);

            default:
                return Ok();
        }
    }
}
