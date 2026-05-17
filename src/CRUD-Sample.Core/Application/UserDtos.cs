namespace CrudSample.Core.Application;

public sealed record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    DateTimeOffset CreatedAt);

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email);

public sealed record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / Math.Max(PageSize, 1));
}
