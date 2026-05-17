namespace CrudSample.Core.Domain;

/// <summary>
/// Domain entity. Construction goes through a factory method so the type
/// cannot be created in an invalid state — the <see cref="Email"/> value
/// object guarantees the address is syntactically valid before the entity
/// even exists.
/// </summary>
public sealed class User
{
    public int Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Required by EF Core.
    private User() { }

    public static User Create(string firstName, string lastName, Email email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        return new User
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Update(string firstName, string lastName, Email email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email;
    }
}
