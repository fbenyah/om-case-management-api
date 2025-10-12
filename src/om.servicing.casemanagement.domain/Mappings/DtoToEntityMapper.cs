using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.domain.Mappings;

/// <summary>
/// Provides methods to map Data Transfer Objects (DTOs) to their corresponding entity models.
/// </summary>
/// <remarks>This static class contains utility methods for converting DTO objects, such as <see
/// cref="OMCaseDto"/>,  <see cref="OMInteractionDto"/>, <see cref="OMTransactionDto"/>, and <see
/// cref="OMTransactionTypeDto"/>,  into their respective entity representations. The mapping ensures that all relevant
/// fields are transferred  and handles null values gracefully by providing default values where necessary.</remarks>
public static class DtoToEntityMapper
{
    /// <summary>
    /// Converts an <see cref="OMCaseDto"/> object to an <see cref="OMCase"/> entity.
    /// </summary>
    /// <remarks>The <see cref="OMCase.CreatedDate"/> and <see cref="OMCase.UpdateDate"/> properties are set
    /// to  <see cref="DateTime.MinValue"/> if the corresponding values in <paramref name="dto"/> are null. The <see
    /// cref="OMCase.Interactions"/> collection is initialized as an empty list if  <paramref name="dto.Interactions"/>
    /// is null.</remarks>
    /// <param name="dto">The data transfer object representing the case to be converted. Cannot be null.</param>
    /// <returns>An <see cref="OMCase"/> entity populated with the data from the specified <paramref name="dto"/>.</returns>
    public static OMCase ToEntity(OMCaseDto dto) => new OMCase
    {
        Id = dto.Id,
        Channel = dto.Channel,
        CreatedDate = dto.CreatedDate != null ? Convert.ToDateTime(dto.CreatedDate) : DateTime.MinValue,
        UpdateDate = dto.UpdateDate != null ? Convert.ToDateTime(dto.UpdateDate) : DateTime.MinValue,
        Status = dto.Status,
        IdentificationNumber = dto.IdentificationNumber,
        Interactions = dto.Interactions?.Select(i => ToEntity(i)).ToList() ?? new List<OMInteraction>()
    };

    /// <summary>
    /// Converts an <see cref="OMInteractionDto"/> object to an <see cref="OMInteraction"/> entity.
    /// </summary>
    /// <remarks>This method performs a mapping from the DTO to the entity, including nested objects such as
    /// cases and transactions. Null values in the DTO are handled gracefully, with default values applied where
    /// necessary.</remarks>
    /// <param name="dto">The data transfer object containing the interaction details to convert.</param>
    /// <returns>An <see cref="OMInteraction"/> entity populated with the data from the specified <paramref name="dto"/>. If any
    /// nullable fields in <paramref name="dto"/> are null, default values are used.</returns>
    public static OMInteraction ToEntity(OMInteractionDto dto) => new OMInteraction
    {
        Id = dto.Id,
        Notes = dto.Notes,
        CreatedDate = dto.CreatedDate != null ? Convert.ToDateTime(dto.CreatedDate) : DateTime.MinValue,
        UpdateDate = dto.UpdateDate != null ? Convert.ToDateTime(dto.UpdateDate) : DateTime.MinValue,
        Status = dto.Status,
        CaseId = dto.Case != null ? dto.Case.Id : string.Empty,
        Case = dto.Case != null ? ToEntity(dto.Case) : null,
        Transactions = dto.Transactions?.Select(t => ToEntity(t)).ToList() ?? new List<OMTransaction>()
    };

    /// <summary>
    /// Converts an <see cref="OMTransactionDto"/> object to an <see cref="OMTransaction"/> entity.
    /// </summary>
    /// <remarks>This method maps the properties of the <see cref="OMTransactionDto"/> to their corresponding
    /// properties in the <see cref="OMTransaction"/> entity. Nested objects, such as <c>TransactionType</c>,
    /// <c>Interaction</c>, and <c>Case</c>, are also converted recursively if they are not <c>null</c>.</remarks>
    /// <param name="dto">The data transfer object containing the transaction details to convert. Cannot be <c>null</c>.</param>
    /// <returns>An <see cref="OMTransaction"/> entity populated with the data from the specified <paramref name="dto"/>.</returns>
    public static OMTransaction ToEntity(OMTransactionDto dto) => new OMTransaction
    {
        Id = dto.Id,
        CreatedDate = dto.CreatedDate != null ? Convert.ToDateTime(dto.CreatedDate) : DateTime.MinValue,
        UpdateDate = dto.UpdateDate != null ? Convert.ToDateTime(dto.UpdateDate) : DateTime.MinValue,
        Status = dto.Status,
        IsImmediate = dto.IsImmediate,
        ProcessedDetails = dto.ProcessedDetails,
        ReceivedDetails = dto.ReceivedDetails,
        TransactionTypeId = dto.TransactionType != null ? dto.TransactionType.Id : string.Empty,
        TransactionType = dto.TransactionType != null ? ToEntity(dto.TransactionType) : null,
        InteractionId = dto.Interaction != null ? dto.Interaction.Id : string.Empty,
        Interaction = dto.Interaction != null ? ToEntity(dto.Interaction) : null,
        CaseId = dto.Case != null ? dto.Case.Id : string.Empty,
        Case = dto.Case != null ? ToEntity(dto.Case) : null
    };

    /// <summary>
    /// Converts an <see cref="OMTransactionTypeDto"/> instance to an <see cref="OMTransactionType"/> entity.
    /// </summary>
    /// <remarks>If <paramref name="dto"/> contains null values for <see
    /// cref="OMTransactionTypeDto.CreatedDate"/> or  <see cref="OMTransactionTypeDto.UpdateDate"/>, the corresponding
    /// properties in the returned entity  will be set to <see cref="DateTime.MinValue"/>.</remarks>
    /// <param name="dto">The data transfer object containing the transaction type details to convert.</param>
    /// <returns>An <see cref="OMTransactionType"/> entity populated with the values from the specified <paramref name="dto"/>.</returns>
    public static OMTransactionType ToEntity(OMTransactionTypeDto dto) => new OMTransactionType
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        CreatedDate = dto.CreatedDate != null ? Convert.ToDateTime(dto.CreatedDate) : DateTime.MinValue,
        UpdateDate = dto.UpdateDate != null ? Convert.ToDateTime(dto.UpdateDate) : DateTime.MinValue,
        RequiresApproval = dto.RequiresApproval
    };
}
