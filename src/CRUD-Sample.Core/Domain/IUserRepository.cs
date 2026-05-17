namespace CrudSample.Core.Domain;

/// <summary>
/// Persistence contract for <see cref="User"/>. Lives in Domain so the
/// Application layer can depend on it without knowing about EF Core.
/// </summary>
public interface IUserRepository
{
    Task<IReadOnlyList<User>> ListAsync(PageRequest page, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<User?> FindAsync(int id, CancellationToken ct);
    Task<bool> EmailExistsAsync(Email email, int? excludeId, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task<bool> RemoveAsync(int id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public readonly record struct PageRequest(int Page, int PageSize, string? SortBy, bool Descending)
{
    public int Skip => Math.Max(0, (Page - 1) * PageSize);

    public static PageRequest Default => new(1, 20, null, false);

    public PageRequest Normalised() =>
        new(
            Math.Max(1, Page),
            Math.Clamp(PageSize, 1, 100),
            SortBy,
            Descending);
}
