using Microsoft.AspNetCore.Mvc;

namespace ProductService.Application.Infrastructure;

public static class ResultExtensions
{
    public static ActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase thisController
    )
    {
        if (result.IsError)
            return HandleError(result.Error, thisController);
        return thisController.Ok(result.Value);
    }

    public static ActionResult ToActionResult<T, V>(
        this Result<T> result,
        ControllerBase thisController,
        Func<T, V> converter
    )
    {
        if (result.IsError)
            return HandleError(result.Error, thisController);
        return thisController.Ok(converter(result.Value));
    }

    public static ActionResult ToActionResult(this Result result, ControllerBase thisController)
    {
        if (result.IsError)
            return HandleError(result.Error, thisController);
        return thisController.NoContent();
    }

    private static ObjectResult HandleError(Error error, ControllerBase thisController) =>
        thisController.StatusCode((int)error.HttpStatus, error.Message);
}
