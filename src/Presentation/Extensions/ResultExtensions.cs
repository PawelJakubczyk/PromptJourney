//using Domain.ResultExtensions;
//using FluentResults;
//using Microsoft.AspNetCore.Mvc;

//namespace Presentation.Extensions;

//public static class ResultExtensions
//{
//    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
//    {
//        if (result.IsSuccess)
//            return controller.Ok(result.Value);

//        if (result.Errors.Any(e => e.IsNotFound()))
//            return controller.NotFound(result.Errors);

//        if (result.Errors.Any(e => e.IsInvalid()))
//            return controller.BadRequest(result.Errors);

//        if (result.Errors.Any(e => e.IsUnauthorized()))
//            return controller.Unauthorized(result.Errors);

//        if (result.Errors.Any(e => e.IsForbidden()))
//            return controller.Forbid();

//        if (result.Errors.Any(e => e.IsConflict()))
//            return controller.Conflict(result.Errors);

//        // Default is 500 Internal Server Error
//        return controller.StatusCode(500, result.Errors);
//    }

//    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
//    {
//        if (result.IsSuccess)
//            return controller.Ok();

//        if (result.Errors.Any(e => e.IsNotFound()))
//            return controller.NotFound(result.Errors);

//        if (result.Errors.Any(e => e.IsInvalid()))
//            return controller.BadRequest(result.Errors);

//        if (result.Errors.Any(e => e.IsUnauthorized()))
//            return controller.Unauthorized(result.Errors);

//        if (result.Errors.Any(e => e.IsForbidden()))
//            return controller.Forbid();

//        if (result.Errors.Any(e => e.IsConflict()))
//            return controller.Conflict(result.Errors);

//        // Default is 500 Internal Server Error
//        return controller.StatusCode(500, result.Errors);
//    }
//}