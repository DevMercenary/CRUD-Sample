using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CrudSample.Core.Domain;

/// <summary>
/// Immutable value object representing a syntactically-valid e-mail address.
/// Equality is case-insensitive on the local-part-and-domain.
/// </summary>
public readonly partial record struct Email
{
    private static readonly Regex Pattern = EmailPattern();

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Parse(string raw)
    {
        if (TryParse(raw, out var email))
        {
            return email;
        }
        throw new ValidationException($"'{raw}' is not a valid e-mail address.");
    }

    public static bool TryParse(string? raw, out Email email)
    {
        email = default;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var trimmed = raw.Trim();
        if (trimmed.Length > 254 || !Pattern.IsMatch(trimmed))
        {
            return false;
        }

        email = new Email(trimmed);
        return true;
    }

    public override string ToString() => Value;

    public bool Equals(Email other) =>
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Value ?? string.Empty);

    [GeneratedRegex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    private static partial Regex EmailPattern();

    public static implicit operator string(Email email) => email.Value;
}
