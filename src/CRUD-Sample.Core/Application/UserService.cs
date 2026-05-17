using CrudSample.Core.Domain;
using Microsoft.Extensions.Logging;

namespace CrudSample.Core.Application;

public sealed partial class UserService(IUserRepository repository, ILogger<UserService> logger) : IUserService
{
    public async Task<PagedResult<UserResponse>> ListAsync(
        int page,
        int pageSize,
        string? sortBy,
        bool desc,
        CancellationToken ct)
    {
        var request = new PageRequest(page, pageSize, sortBy, desc).Normalised();
        var users = await repository.ListAsync(request, ct);
        var total = await repository.CountAsync(ct);

        return new PagedResult<UserResponse>(
            users.Select(ToResponse).ToList(),
            request.Page,
            request.PageSize,
            total);
    }

    public async Task<UserResponse?> GetAsync(int id, CancellationToken ct)
    {
        var user = await repository.FindAsync(id, ct);
        return user is null ? null : ToResponse(user);
    }

    public async Task<UserResult> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        // Validators upstream guarantee the e-mail parses, so this is a sanity check.
        if (!Email.TryParse(request.Email, out var email))
        {
            return new UserResult.EmailConflict(request.Email);
        }

        if (await repository.EmailExistsAsync(email, excludeId: null, ct))
        {
            LogEmailConflict(email.Value);
            return new UserResult.EmailConflict(email.Value);
        }

        var user = User.Create(request.FirstName, request.LastName, email);
        await repository.AddAsync(user, ct);
        await repository.SaveChangesAsync(ct);

        LogCreated(user.Id);
        return new UserResult.Success(ToResponse(user));
    }

    public async Task<UserResult> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await repository.FindAsync(id, ct);
        if (user is null)
        {
            return new UserResult.NotFound();
        }

        if (!Email.TryParse(request.Email, out var email))
        {
            return new UserResult.EmailConflict(request.Email);
        }

        if (await repository.EmailExistsAsync(email, excludeId: id, ct))
        {
            return new UserResult.EmailConflict(email.Value);
        }

        user.Update(request.FirstName, request.LastName, email);
        await repository.SaveChangesAsync(ct);
        return new UserResult.Success(ToResponse(user));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var removed = await repository.RemoveAsync(id, ct);
        if (removed)
        {
            await repository.SaveChangesAsync(ct);
        }
        return removed;
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.FirstName, user.LastName, user.Email.Value, user.CreatedAt);

    // Compile-time generated, allocation-free log delegates.

    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Created user {Id}")]
    private partial void LogCreated(int id);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "Email {Email} already taken")]
    private partial void LogEmailConflict(string email);
}
