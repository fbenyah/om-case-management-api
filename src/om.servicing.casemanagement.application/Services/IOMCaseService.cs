using om.servicing.casemanagement.application.Services.Models;
using om.servicing.casemanagement.domain.Dtos;

namespace om.servicing.casemanagement.application.Services;

public interface IOMCaseService
{
    /// <summary>
    /// Retrieves a list of cases associated with the specified identification number.
    /// </summary>
    /// <remarks>This method queries the case repository to retrieve cases matching the provided
    /// identification number.  If the identification number is invalid, the response will indicate the error without
    /// performing the query. In the event of an exception during the query, the response will include a custom
    /// exception with details about the failure.</remarks>
    /// <param name="identificationNumber">The identification number of the customer whose cases are to be retrieved.  This value cannot be null, empty, or
    /// consist only of whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases associated with the specified identification
    /// number. If the identification number is invalid or an error occurs, the response will include an appropriate
    /// error message or exception.</returns>
    Task<OMCaseListResponse> GetCasesForCustomerByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of cases for a customer based on their identification number and case status.
    /// </summary>
    /// <remarks>If either <paramref name="identityNumber"/> or <paramref name="status"/> is null, empty, or
    /// whitespace,  the response will include an error message indicating that both parameters are required. In the
    /// event of an exception during data retrieval, the response will include a custom exception with details about the
    /// error.</remarks>
    /// <param name="identityNumber">The identification number of the customer. Cannot be null, empty, or whitespace.</param>
    /// <param name="status">The status of the cases to retrieve. Cannot be null, empty, or whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases matching the specified identification number
    /// and status. If no cases are found, the <see cref="OMCaseListResponse.Data"/> property will contain an empty
    /// list.</returns>
    Task<OMCaseListResponse> GetCasesForCustomerByIdentificationNumberAndStatusAsync(string identityNumber, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of cases associated with the specified reference number.
    /// </summary>
    /// <remarks>This method queries the case repository for cases that match the provided reference number.
    /// If the reference number is invalid, the response will include an error message indicating the issue. In the
    /// event of an exception during the operation, the response will include a custom exception with details about the
    /// error, and the error will be logged.</remarks>
    /// <param name="referenceNumber">The reference number used to identify the cases. This value cannot be null, empty, or consist only of
    /// whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases matching the specified reference number. If no
    /// cases are found, the <see cref="OMCaseListResponse.Data"/> property will be an empty list. If an error occurs,
    /// the response will include an appropriate error message or exception.</returns>
    Task<OMCaseListResponse> GetCasesForCustomerByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of cases for a customer based on the specified reference number and status.
    /// </summary>
    /// <remarks>This method queries the case repository to find cases that match the provided reference
    /// number and status. If either <paramref name="referenceNumber"/> or <paramref name="status"/> is null, empty, or
    /// whitespace, the method returns a response with an error message indicating that both parameters are required. In
    /// the event of an exception during the query, the method logs the error and returns a response with a custom
    /// exception.</remarks>
    /// <param name="referenceNumber">The unique reference number associated with the customer. This value cannot be null, empty, or whitespace.</param>
    /// <param name="status">The status of the cases to retrieve. This value cannot be null, empty, or whitespace.</param>
    /// <returns>An <see cref="OMCaseListResponse"/> containing the list of cases that match the specified reference number and
    /// status. If no cases are found, the <see cref="OMCaseListResponse.Data"/> property will contain an empty list. If
    /// an error occurs, the response will include an appropriate error message or exception.</returns>
    Task<OMCaseListResponse> GetCasesForCustomerByReferenceNumberAndStatusAsync(string referenceNumber, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new case asynchronously based on the provided case data transfer object (DTO).
    /// </summary>
    /// <remarks>This method generates a unique identifier and reference number for the case, maps the DTO to
    /// an  entity, and persists the entity to the repository. If an error occurs during persistence, the  response will
    /// include a custom exception with details about the failure.</remarks>
    /// <param name="omCaseDto">The data transfer object containing the details of the case to be created.  This parameter cannot be <see
    /// langword="null"/>.</param>
    /// <returns>An <see cref="OMCaseCreateResponse"/> containing the reference number and ID of the created case.  If the input
    /// is <see langword="null"/> or an error occurs during case creation, the response will  indicate the failure and
    /// include relevant error details.</returns>
    Task<OMCaseCreateResponse> CreateCaseAsync(OMCaseDto omCaseDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether a case with the specified ID exists in the repository.
    /// </summary>
    /// <remarks>This method logs an error if an exception occurs while accessing the repository.</remarks>
    /// <param name="caseId">The unique identifier of the case to check. Cannot be null, empty, or whitespace.</param>
    /// <returns><see langword="true"/> if a case with the specified ID exists; otherwise, <see langword="false"/>. Returns <see
    /// langword="false"/> if the <paramref name="caseId"/> is null, empty, or whitespace, or if an error occurs during
    /// the operation.</returns>
    Task<bool> CaseExistsWithIdAsync(string caseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether a case with the specified reference number exists.
    /// </summary>
    /// <remarks>If the <paramref name="referenceNumber"/> is null, empty, or consists only of whitespace, the
    /// method immediately returns <see langword="false"/>. Logs an error and returns <see langword="false"/> if an
    /// exception occurs during the operation.</remarks>
    /// <param name="referenceNumber">The reference number of the case to check. Cannot be null, empty, or whitespace.</param>
    /// <returns><see langword="true"/> if a case with the specified reference number exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> CaseExistsWithReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
}
