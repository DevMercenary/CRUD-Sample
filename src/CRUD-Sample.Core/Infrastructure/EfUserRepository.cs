using CrudSample.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace CrudSample.Core.Infrastructure;

public sealed class EfUserRepository(AppDbContext db) : IUserRepository
{
    public async Task<IReadOnlyList<User>> ListAsync(PageRequest page, CancellationToken ct)
    {
        var query = db.Users.AsNoTracking();
        query = page.SortBy?.ToLowerInvariant() switch
        {
            "firstname" => page.Descending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => page.Descending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "email" => page.Descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "createdat" => page.Descending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            _ => page.Descending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
        };

        return await query.Skip(page.Skip).Take(page.PageSize).ToListAsync(ct);
    }

    public Task<int> CountAsync(CancellationToken ct) =>
        db.Users.CountAsync(ct);

    public Task<User?> FindAsync(int id, CancellationToken ct) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<bool> EmailExistsAsync(Email email, int? excludeId, CancellationToken ct)
    {
        var query = db.Users.AsQueryable();
        if (excludeId is { } id)
        {
            query = query.Where(u => u.Id != id);
        }
        return await query.AnyAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct) =>
        await db.Users.AddAsync(user, ct);

    public async Task<bool> RemoveAsync(int id, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([id], ct);
        if (user is null)
        {
            return false;
        }
        db.Users.Remove(user);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
