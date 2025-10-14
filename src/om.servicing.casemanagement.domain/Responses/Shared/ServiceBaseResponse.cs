using System.Text.Json.Serialization;

namespace om.servicing.casemanagement.domain.Responses.Shared;

public abstract class ServiceBaseResponse<T> : BaseFluentValidationError
{
    public DateTime ResponseTime { get; private set; } = DateTime.Now;

    [JsonPropertyName("data")]
    public T Data { get; set; }
}
