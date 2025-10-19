using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;

namespace om.servicing.casemanagement.domain.Mappings;

/// <summary>
/// Provides methods for mapping entity objects to their corresponding Data Transfer Object (DTO) representations.
/// </summary>
/// <remarks>This static class contains a collection of methods that convert various entity types, such as <see
/// cref="OMCase"/>,  <see cref="OMInteraction"/>, <see cref="OMTransaction"/>, and <see cref="OMTransactionType"/>,
/// into their respective  DTO types (<see cref="OMCaseDto"/>, <see cref="OMInteractionDto"/>, <see
/// cref="OMTransactionDto"/>, and  <see cref="OMTransactionTypeDto"/>). These methods are designed to simplify the
/// transformation of domain entities  into lightweight DTOs for use in application layers such as APIs or services. 
/// Each mapping method ensures that nested or related entities are also mapped to their DTO equivalents, handling  null
/// references gracefully by returning default values (e.g., empty lists or null) where appropriate.</remarks>
public static class EntityToDtoMapper
{
    /// <summary>
    /// Converts an <see cref="OMCase"/> entity to its corresponding <see cref="OMCaseDto"/> representation.
    /// </summary>
    /// <param name="entity">The <see cref="OMCase"/> entity to convert. Cannot be <see langword="null"/>.</param>
    /// <returns>An <see cref="OMCaseDto"/> object containing the mapped data from the specified <see cref="OMCase"/> entity.</returns>
    public static OMCaseDto ToDto(OMCase entity) => new OMCaseDto
    {
        Id = entity.Id,
        Channel = entity.Channel,
        CreatedDate = entity.CreatedDate,
        UpdateDate = entity.UpdateDate,
        Status = entity.Status,
        IdentificationNumber = entity.IdentificationNumber,
        ReferenceNumber = entity.ReferenceNumber,
        Interactions = entity.Interactions?.Select(i => ToDto(i)).ToList() ?? new List<OMInteractionDto>()
    };

    /// <summary>
    /// Converts an <see cref="OMInteraction"/> entity to its corresponding data transfer object (DTO).
    /// </summary>
    /// <param name="entity">The <see cref="OMInteraction"/> entity to convert. Cannot be <see langword="null"/>.</param>
    /// <returns>An <see cref="OMInteractionDto"/> representing the converted entity. If the entity contains related objects,
    /// they are also converted to their respective DTOs.</returns>
    public static OMInteractionDto ToDto(OMInteraction entity) => new OMInteractionDto
    {
        Id = entity.Id,
        Notes = entity.Notes,
        CreatedDate = entity.CreatedDate,
        UpdateDate = entity.UpdateDate,
        Status = entity.Status,
        ReferenceNumber = entity.ReferenceNumber,
        Case = entity.Case != null ? ToDto(entity.Case) : null,
        Transactions = entity.Transactions?.Select(t => ToDto(t)).ToList() ?? new List<OMTransactionDto>()
    };

    /// <summary>
    /// Converts an <see cref="OMTransaction"/> entity to its corresponding <see cref="OMTransactionDto"/>
    /// representation.
    /// </summary>
    /// <param name="entity">The <see cref="OMTransaction"/> entity to convert. Cannot be <see langword="null"/>.</param>
    /// <returns>An <see cref="OMTransactionDto"/> instance containing the mapped data from the specified <see
    /// cref="OMTransaction"/> entity.</returns>
    public static OMTransactionDto ToDto(OMTransaction entity) => new OMTransactionDto
    {
        Id = entity.Id,
        CreatedDate = entity.CreatedDate,
        UpdateDate = entity.UpdateDate,
        Status = entity.Status,
        IsImmediate = entity.IsImmediate,
        IsFulfilledExternally = entity.IsFulfilledExternally,
        ExternalSystem = entity.ExternalSystem,
        ExternalSystemId = entity.ExternalSystemId,
        ReferenceNumber = entity.ReferenceNumber,
        ProcessedDetails = entity.ProcessedDetails,
        ReceivedDetails = entity.ReceivedDetails,
        TransactionType = entity.TransactionType != null ? ToDto(entity.TransactionType) : null,
        Interaction = entity.Interaction != null ? ToDto(entity.Interaction) : null,
        Case = entity.Case != null ? ToDto(entity.Case) : null
    };

    /// <summary>
    /// Converts an <see cref="OMTransactionType"/> entity to its corresponding data transfer object (DTO).
    /// </summary>
    /// <param name="entity">The <see cref="OMTransactionType"/> entity to convert. Cannot be <see langword="null"/>.</param>
    /// <returns>An <see cref="OMTransactionTypeDto"/> instance containing the data from the specified entity.</returns>
    public static OMTransactionTypeDto ToDto(OMTransactionType entity) => new OMTransactionTypeDto
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        CreatedDate = entity.CreatedDate,
        UpdateDate = entity.UpdateDate,
        RequiresApproval = entity.RequiresApproval
    };
}
