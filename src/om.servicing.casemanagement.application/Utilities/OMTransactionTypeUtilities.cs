using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMTransactionTypeUtilities
{
    /// <summary>
    /// Converts a collection of <see cref="OMTransactionType"/> objects to a list of <see cref="OMTransactionTypeDto"/>
    /// objects.
    /// </summary>
    /// <param name="omTransactionTypes">The collection of <see cref="OMTransactionType"/> objects to convert. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMTransactionTypeDto"/> objects. Returns an empty list if <paramref
    /// name="omTransactionTypes"/> is null or contains no elements.</returns>
    public static List<OMTransactionTypeDto> ReturnTransactionTypeDtoList(IEnumerable<OMTransactionType> omTransactionTypes)
    {
        List<OMTransactionTypeDto> transactionTypeDtoList = new List<OMTransactionTypeDto>();

        if (omTransactionTypes == null || !omTransactionTypes.Any())
        {
            return transactionTypeDtoList;
        }

        foreach (OMTransactionType omTransactionType in omTransactionTypes)
        {
            transactionTypeDtoList.Add(EntityToDtoMapper.ToDto(omTransactionType));
        }

        return transactionTypeDtoList;
    }

    /// <summary>
    /// Converts a list of transaction type DTOs to a list of transaction type entities.
    /// </summary>
    /// <param name="transactionTypeDtoList">A list of <see cref="OMTransactionTypeDto"/> objects to be converted.  If the list is <see langword="null"/> or
    /// empty, an empty list is returned.</param>
    /// <returns>A list of <see cref="OMTransactionType"/> objects converted from the provided DTOs.  Returns an empty list if
    /// <paramref name="transactionTypeDtoList"/> is <see langword="null"/> or empty.</returns>
    public static List<OMTransactionType> ReturnTransactionTypeList(List<OMTransactionTypeDto> transactionTypeDtoList)
    {
        List<OMTransactionType> transactionTypeList = new List<OMTransactionType>();

        if (transactionTypeDtoList == null || !transactionTypeDtoList.Any())
        {
            return transactionTypeList;
        }

        foreach (OMTransactionTypeDto omTransactionTypeDto in transactionTypeDtoList)
        {
            transactionTypeList.Add(DtoToEntityMapper.ToEntity(omTransactionTypeDto));
        }

        return transactionTypeList;
    }
}
