using CrudSample.Core.Application;
using Microsoft.AspNetCore.Mvc;

namespace CrudSample.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public sealed class UsersController(IUserService users) : ControllerBase
{
    /// <summary>Paginated list of users.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool desc = false,
        CancellationToken ct = default)
    {
        var result = await users.ListAsync(page, pageSize, sortBy, desc, ct);
        return Ok(result);
    }

    /// <summary>Get a single user by id.</summary>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Get(int id, CancellationToken ct)
    {
        var user = await users.GetAsync(id, ct);
        return user is null ? UserNotFound(id) : Ok(user);
    }

    /// <summary>Create a new user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var result = await users.CreateAsync(request, ct);
        return result switch
        {
            UserResult.Success s => CreatedAtAction(nameof(Get), new { id = s.User.Id }, s.User),
            UserResult.EmailConflict ec => EmailConflict(ec.Email),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    /// <summary>Update an existing user.</summary>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Update(
        int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct)
    {
        var result = await users.UpdateAsync(id, request, ct);
        return result switch
        {
            UserResult.Success s => Ok(s.User),
            UserResult.NotFound => UserNotFound(id),
            UserResult.EmailConflict ec => EmailConflict(ec.Email),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    /// <summary>Delete a user.</summary>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var removed = await users.DeleteAsync(id, ct);
        return removed ? NoContent() : UserNotFound(id);
    }

    private ObjectResult UserNotFound(int id) =>
        Problem(
            detail: $"No user exists with id {id}.",
            statusCode: StatusCodes.Status404NotFound,
            title: "User not found",
            type: "https://crud-sample/errors/user-not-found");

    private ObjectResult EmailConflict(string email) =>
        Problem(
            detail: $"The address '{email}' is already registered.",
            statusCode: StatusCodes.Status409Conflict,
            title: "Email already in use",
            type: "https://crud-sample/errors/email-conflict");
}
