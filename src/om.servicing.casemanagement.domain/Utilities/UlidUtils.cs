namespace om.servicing.casemanagement.domain.Utilities;

/// <summary>
/// Provides utility methods for working with ULIDs (Universally Unique Lexicographically Sortable Identifiers).
/// </summary>
/// <remarks>This class includes methods for generating, validating, parsing, and converting ULIDs. ULIDs are
/// 26-character, case-insensitive alphanumeric strings that are lexicographically sortable and suitable for use as
/// unique identifiers in distributed systems. All methods in this class are thread-safe.</remarks>
public static class UlidUtils
{
    /// <summary>
    /// Generates a new ULID (Universally Unique Lexicographically Sortable Identifier) as a string.
    /// </summary>
    /// <remarks>The generated ULID is a 26-character, case-insensitive string that is lexicographically
    /// sortable. This method is thread-safe and can be used to generate unique identifiers in distributed
    /// systems.</remarks>
    /// <returns>A string representation of a newly generated ULID.</returns>
    public static string NewUlidString()
    {
        return Ulid.NewUlid().ToString();
    }

    /// <summary>
    /// Determines whether the specified string is a valid ULID (Universally Unique Lexicographically Sortable
    /// Identifier).
    /// </summary>
    /// <remarks>A valid ULID is a 26-character, case-insensitive alphanumeric string that conforms to the
    /// ULID specification. This method returns <see langword="false"/> if the input is <see langword="null"/>, empty,
    /// or contains only whitespace.</remarks>
    /// <param name="ulidString">The string to validate as a ULID.</param>
    /// <returns><see langword="true"/> if the specified string is a valid ULID; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidUlid(string ulidString)
    {
        if (string.IsNullOrWhiteSpace(ulidString))
            return false;

        try
        {
            var _ = Ulid.Parse(ulidString);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse the specified string representation of a ULID (Universally Unique Lexicographically Sortable
    /// Identifier).
    /// </summary>
    /// <remarks>This method does not throw exceptions for invalid input. Instead, it returns <see
    /// langword="false"/> and sets <paramref name="ulid"/> to its default value if the parsing fails.</remarks>
    /// <param name="ulidString">The string representation of the ULID to parse.</param>
    /// <param name="ulid">When this method returns, contains the parsed <see cref="Ulid"/> value if the parsing succeeded; otherwise,
    /// contains the default value of <see cref="Ulid"/>.</param>
    /// <returns><see langword="true"/> if the string was successfully parsed into a valid <see cref="Ulid"/>; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool TryParseUlid(string ulidString, out Ulid ulid)
    {
        try
        {
            ulid = Ulid.Parse(ulidString);
            return true;
        }
        catch
        {
            ulid = default;
            return false;
        }
    }

    /// <summary>
    /// Converts the specified <see cref="Ulid"/> to its equivalent <see cref="Guid"/> representation.
    /// </summary>
    /// <param name="ulid">The <see cref="Ulid"/> to convert.</param>
    /// <returns>A <see cref="Guid"/> that represents the same value as the specified <see cref="Ulid"/>.</returns>
    public static Guid UlidToGuid(Ulid ulid)
    {
        return ulid.ToGuid();
    }
}
