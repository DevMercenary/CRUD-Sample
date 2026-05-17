namespace CrudSample.Core.Application;

/// <summary>
/// Application-layer use cases. The API layer talks to this — it never reaches
/// for the repository directly. Returns domain-shaped results that the API
/// translates into HTTP responses.
/// </summary>
public interface IUserService
{
    Task<PagedResult<UserResponse>> ListAsync(int page, int pageSize, string? sortBy, bool desc, CancellationToken ct);
    Task<UserResponse?> GetAsync(int id, CancellationToken ct);
    Task<UserResult> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserResult> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}

public abstract record UserResult
{
    public sealed record Success(UserResponse User) : UserResult;
    public sealed record NotFound : UserResult;
    public sealed record EmailConflict(string Email) : UserResult;
}
