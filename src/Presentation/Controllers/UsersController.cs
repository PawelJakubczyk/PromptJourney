using Application.UseCases.Common.Responses;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Queries;
using Application.UseCases.Users.Query;
using Application.UseCases.Users.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.Pipeline;
using static Domain.ValueObjects.Role;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(ISender sender) : ApiController(sender)
{
    // Queries //

    // GET api/users/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<Results<Ok<UserResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserById.Query(id);

        var user = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<UserResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return user;
    }

    // GET api/users/by-email/{email}
    [HttpGet("by-email/{email}")]
    [Authorize(Policy = AdminAccess)]
    public async Task<Results<Ok<UserResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByEmail(
        string email,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByEmail.Query(email);

        var user = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<UserResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return user;
    }

    // GET api/users/by-name/{userName}
    [HttpGet("by-name/{userName}")]
    [Authorize]
    public async Task<Results<Ok<UserResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByName(
        string userName,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByName.Query(userName);

        var user = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<UserResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return user;
    }

    // GET api/users/{id}/exists
    [HttpGet("{id}/exists")]
    [Authorize(Policy = AdminAccess)]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckIdExists(
        string id,
        CancellationToken cancellationToken)
    {
        var query = new CheckUserIdExists.Query(id);

        var exists = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(payload))
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exists;
    }

    // GET api/users/email/{email}/exists
    [HttpGet("email/{email}/exists")]
    [Authorize(Policy = AdminAccess)]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckEmailExists(
        string email,
        CancellationToken cancellationToken)
    {
        var query = new CheckUserEmailExists.Query(email);

        var exists = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(payload))
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exists;
    }

    // GET api/users/name/{userName}/exists
    [HttpGet("name/{userName}/exists")]
    [Authorize(Policy = AdminAccess)]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckNameExists(
        string userName,
        CancellationToken cancellationToken)
    {
        var query = new CheckUserNameExists.Query(userName);

        var exists = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(payload))
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exists;
    }

    // Commands //

    // POST api/users/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<Results<Created<UserResponse>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUser.Command(request.UserName, request.Email, request.Password);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareCreateResponse()
            .ToResultsCreatedAsync<UserResponse, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>
            (
                locationFactory: user => $"/api/users/{user?.UserId}",
                httpContext: HttpContext
            );

        return result;
    }

    // POST api/users/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<Results<Ok<LoginResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginUser.Command(request.Email, request.Password);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<LoginResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // PUT api/users/{id}/email
    [HttpPut("{id}/email")]
    [Authorize]
    public async Task<Results<Ok<UserResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdateEmail(
        string id,
        [FromBody] UpdateUserEmailRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserEmail.Command(id, request.NewEmail);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<UserResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // PUT api/users/{id}/password
    [HttpPut("{id}/password")]
    [Authorize]
    public async Task<Results<Ok<UserResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdatePassword(
        string id,
        [FromBody] UpdateUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserPassword.Command(id, request.NewPassword);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<UserResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // PUT api/users/{id}/role
    [HttpPut("{id}/role")]
    [Authorize(Policy = AdminAccess)]
    public async Task<Results<Ok<UserResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdateRole(
        string id,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserRole.Command(id, request.NewRole);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<UserResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // DELETE api/users/{id}
    [HttpDelete("{id}")]
    [Authorize(Policy = AdminAccess)]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> DeleteById(
        string id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserById.Command(id);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<DeleteResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // DELETE api/users/by-email/{email}
    [HttpDelete("by-email/{email}")]
    [Authorize(Policy = AdminAccess)]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> DeleteByEmail(
        string email,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserByEmail.Command(email);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<DeleteResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // DELETE api/users/{id}/soft
    [HttpDelete("{id}/soft")]
    [Authorize]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> SoftDeleteById(
        string id,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteUserById.Command(id);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<DeleteResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // DELETE api/users/by-email/{email}/soft
    [HttpDelete("by-email/{email}/soft")]
    [Authorize]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> SoftDeleteByEmail(
        string email,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteUserByEmail.Command(email);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<DeleteResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }
}

// Request DTOs
public sealed record RegisterUserRequest(string UserName, string Email, string Password);
public sealed record LoginUserRequest(string Email, string Password);
public sealed record UpdateUserEmailRequest(string NewEmail);
public sealed record UpdateUserPasswordRequest(string NewPassword);
public sealed record UpdateUserRoleRequest(string NewRole);