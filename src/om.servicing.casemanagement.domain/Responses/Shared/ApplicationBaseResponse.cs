using System.Text.Json.Serialization;

namespace om.servicing.casemanagement.domain.Responses.Shared;

public abstract class ApplicationBaseResponse<T> : BaseFluentValidationError
{
    [JsonPropertyName("data")]
    public T Data { get; set; }
}
