using System.Text.Json.Serialization;

namespace om.servicing.casemanagement.domain.Responses.Shared;

/// <summary>
/// Represents a base class for service responses, encapsulating the response data and metadata.
/// </summary>
/// <remarks>This class provides a common structure for service responses, including the response data and the
/// time  the response was generated. It is intended to be used as a base class for more specific response
/// types.</remarks>
/// <typeparam name="T">The type of the data contained in the response.</typeparam>
public abstract class ServiceBaseResponse<T> : BaseFluentValidationError
{
    public DateTime ResponseTime { get; private set; } = DateTime.Now;

    [JsonPropertyName("data")]
    public T Data { get; set; }
}
