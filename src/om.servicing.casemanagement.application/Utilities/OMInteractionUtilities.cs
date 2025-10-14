using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMInteractionUtilities
{
    /// <summary>
    /// Converts a collection of <see cref="OMInteraction"/> objects to a list of <see cref="OMInteractionDto"/>
    /// objects.
    /// </summary>
    /// <param name="omInteractions">The collection of <see cref="OMInteraction"/> objects to convert. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMInteractionDto"/> objects. Returns an empty list if <paramref name="omInteractions"/> is
    /// null or contains no elements.</returns>
    public static List<OMInteractionDto> ReturnInteractionDtoList(IEnumerable<OMInteraction> omInteractions)
    {
        List<OMInteractionDto> interactionDtoList = new List<OMInteractionDto>();

        if (omInteractions == null || !omInteractions.Any())
        {
            return interactionDtoList;
        }

        foreach (OMInteraction omInteraction in omInteractions)
        {
            interactionDtoList.Add(EntityToDtoMapper.ToDto(omInteraction));
        }

        return interactionDtoList;
    }

    /// <summary>
    /// Converts a list of interaction DTOs to a list of interaction entities.
    /// </summary>
    /// <param name="interactionDtoList">A list of <see cref="OMInteractionDto"/> objects to be converted.  If the list is null or empty, an empty list
    /// is returned.</param>
    /// <returns>A list of <see cref="OMInteraction"/> objects converted from the provided DTOs.  Returns an empty list if
    /// <paramref name="interactionDtoList"/> is null or empty.</returns>
    public static List<OMInteraction> ReturnInteractionList(List<OMInteractionDto> interactionDtoList)
    {
        List<OMInteraction> interactionList = new List<OMInteraction>();

        if (interactionDtoList == null || !interactionDtoList.Any())
        {
            return interactionList;
        }

        foreach (OMInteractionDto omInteractionDto in interactionDtoList)
        {
            interactionList.Add(DtoToEntityMapper.ToEntity(omInteractionDto));
        }

        return interactionList;
    }
}
