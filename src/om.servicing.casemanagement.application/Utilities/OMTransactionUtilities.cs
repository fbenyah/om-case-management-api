using om.servicing.casemanagement.domain.Dtos;
using om.servicing.casemanagement.domain.Entities;
using om.servicing.casemanagement.domain.Mappings;

namespace om.servicing.casemanagement.application.Utilities;

public static class OMTransactionUtilities
{
    /// <summary>
    /// Converts a collection of <see cref="OMTransaction"/> objects to a list of <see cref="OMTransactionDto"/>
    /// objects.
    /// </summary>
    /// <param name="omTransactions">The collection of <see cref="OMTransaction"/> objects to convert. Can be null or empty.</param>
    /// <returns>A list of <see cref="OMTransactionDto"/> objects. Returns an empty list if <paramref name="omTransactions"/> is
    /// null or contains no elements.</returns>
    public static List<OMTransactionDto> ReturnTransactionDtoList(IEnumerable<OMTransaction> omTransactions)
    {
        List<OMTransactionDto> transactionDtoList = new List<OMTransactionDto>();

        if (omTransactions == null || !omTransactions.Any())
        {
            return transactionDtoList;
        }

        foreach (OMTransaction omTransaction in omTransactions)
        {
            transactionDtoList.Add(EntityToDtoMapper.ToDto(omTransaction));
        }

        return transactionDtoList;
    }

    /// <summary>
    /// Converts a list of transaction DTOs to a list of transaction entities.
    /// </summary>
    /// <param name="transactionDtoList">A list of <see cref="OMTransactionDto"/> objects to be converted.  If the list is null or empty, an empty list
    /// is returned.</param>
    /// <returns>A list of <see cref="OMTransaction"/> objects converted from the provided DTOs.  Returns an empty list if
    /// <paramref name="transactionDtoList"/> is null or contains no elements.</returns>
    public static List<OMTransaction> ReturnTransactionList(List<OMTransactionDto> transactionDtoList)
    {
        List<OMTransaction> transactionList = new List<OMTransaction>();

        if (transactionDtoList == null || !transactionDtoList.Any())
        {
            return transactionList;
        }

        foreach (OMTransactionDto omTransactionDto in transactionDtoList)
        {
            transactionList.Add(DtoToEntityMapper.ToEntity(omTransactionDto));
        }

        return transactionList;
    }
}
