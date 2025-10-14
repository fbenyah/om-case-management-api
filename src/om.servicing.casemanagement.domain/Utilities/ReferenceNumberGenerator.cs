using om.servicing.casemanagement.domain.Enums;

namespace om.servicing.casemanagement.domain.Utilities;

/// <summary>
/// Provides functionality to generate unique reference numbers based on specified parameters such as ULID, case
/// channel, and operational business segment.
/// </summary>
/// <remarks>This class is designed to generate reference numbers that are unique and traceable. The generated
/// reference number includes a prefix derived from the operational business segment and case channel, a timestamp, a
/// random code, and a suffix derived from the provided ULID.</remarks>
public static class ReferenceNumberGenerator
{
    /// <summary>
    /// Generates a unique reference number based on the provided ULID, case channel, and operational business segment.
    /// </summary>
    /// <param name="ulid">A unique identifier string. Must be at least 6 characters long and cannot be null, empty, or whitespace.</param>
    /// <param name="channel">The case channel used to determine the channel prefix. Must not be <see cref="CaseChannel.Unknown"/>.</param>
    /// <param name="operationalBusinessSegment">The operational business segment used to determine the segment prefix. Defaults to <see
    /// cref="OperationalBusinessSegment.CustomerServicing"/> if not specified.</param>
    /// <returns>A unique reference number string composed of the operational business segment prefix, channel prefix, timestamp,
    /// random code, and the last 6 characters of the ULID.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="ulid"/> is null, empty, whitespace, or shorter than 6 characters,  or if <paramref
    /// name="channel"/> is <see cref="CaseChannel.Unknown"/>.</exception>
    public static string GenerateReferenceNumber(string ulid, CaseChannel channel, OperationalBusinessSegment operationalBusinessSegment = OperationalBusinessSegment.CustomerServicing)
    {
        if (string.IsNullOrWhiteSpace(ulid) || channel.Equals(CaseChannel.Unknown) || ulid.Length < 6)
        {
            throw new ArgumentException("Invalid ULID or channel");
        }

        string channelPrefix = GetChannelPrefix(channel);
        string obsPrefix = GetOperationalBusinessSegmentPrefix(operationalBusinessSegment);

        // Get the last 6 characters of the ULID
        string ulidSuffix = ulid[^6..];

        // Format timestamp: YYMMDDHHMMSS → compact but still traceable
        string timestamp = DateTime.UtcNow.ToString("yyMMdd");

        // 3 digit random code to ensure uniqueness if multiple cases are created in the same second
        string randomCode = new Random().Next(100, 999).ToString(); 

        return $"{obsPrefix}{channelPrefix}{timestamp}{randomCode}{ulidSuffix}";
    }

    /// <summary>
    /// Retrieves the prefix string associated with the specified <see cref="CaseChannel"/>.
    /// </summary>
    /// <param name="channel">The <see cref="CaseChannel"/> value for which to retrieve the prefix.</param>
    /// <returns>A string representing the prefix associated with the specified <see cref="CaseChannel"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="channel"/> value is not a valid <see cref="CaseChannel"/>.</exception>
    internal static string GetChannelPrefix(CaseChannel channel)
    {
        return channel switch
        {
            CaseChannel.AdviserWorkBench => "D",
            CaseChannel.AgentWorkBench => "T",
            CaseChannel.Branch => "B",
            CaseChannel.Connect => "C",
            CaseChannel.MomApp => "A",
            CaseChannel.PublicWeb => "P",
            CaseChannel.SecureWeb => "W",
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
    }

    /// <summary>
    /// Retrieves the prefix associated with the specified operational business segment.
    /// </summary>
    /// <param name="operationalBusinessSegment">The operational business segment for which to retrieve the prefix.</param>
    /// <returns>A string representing the prefix for the specified operational business segment.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified <paramref name="operationalBusinessSegment"/> is not a recognized value.</exception>
    internal static string GetOperationalBusinessSegmentPrefix(OperationalBusinessSegment operationalBusinessSegment)
    {
        return operationalBusinessSegment switch
        {
            OperationalBusinessSegment.CustomerServicing => "CS",
            _ => throw new ArgumentOutOfRangeException(nameof(operationalBusinessSegment), operationalBusinessSegment, null)
        };
    }
}
