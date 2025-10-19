using Microsoft.AspNetCore.Mvc;
using om.servicing.casemanagement.application.Features.OMCases.Commands;
using om.servicing.casemanagement.application.Features.OMCases.Queries;
using om.servicing.casemanagement.domain.Constants;
using om.servicing.casemanagement.domain.Enums;
using om.servicing.casemanagement.domain.Responses.Shared;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace om.servicing.casemanagement.api.Controllers.V1;

[ApiController]
[Route("/api/casemanagement/v{version:apiVersion}/[controller]")]
public class CaseContoller : BaseController
{
    [SwaggerOperation(
        Summary = "Customer cases by indentification number.",
        Description = @"This request gets all customer cases given the identification number linked to that case.")]
    [HttpGet]
    [Route("by/identification/{identificationNumber}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomerCasesByIdentificationNumberResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetCustomerCasesByIdentification(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem, 
        string identificationNumber)
    {
        GetCustomerCasesByIdentificationNumberResponse response = await Mediator.Send(new GetCustomerCasesByIdentificationNumberQuery
        {
            IdentificationNumber = identificationNumber
        });

        return HandleApplicationEnterpriseResponse<GetCustomerCasesByIdentificationNumberResponse>(response);
    }

    [SwaggerOperation(
        Summary = "Customer cases by reference number.",
        Description = @"This request gets all customer cases given the reference number linked to that case.")]
    [HttpGet]
    [Route("by/reference/{referenceNumber}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomerCasesByReferenceNumberResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetCustomerCasesByReference(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem,
        string referenceNumber)
    {
        GetCustomerCasesByReferenceNumberResponse response = await Mediator.Send(new GetCustomerCasesByReferenceNumberQuery
        {
            ReferenceNumber = referenceNumber
        });

        return HandleApplicationEnterpriseResponse<GetCustomerCasesByReferenceNumberResponse>(response);
    }

    [SwaggerOperation(
        Summary = "Customer cases by indentification number and case status.",
        Description = @"This request gets all customer cases given a particular status and the identification number linked to that case.")]
    [HttpGet]
    [Route("by/identification/{identificationNumber}/status/{status}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomerCasesByIdentificationNumberAndStatusResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetCustomerCasesByIdentificationAndStatus(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem,
        string identificationNumber,
        string status)
    {
        GetCustomerCasesByIdentificationNumberAndStatusResponse response = await Mediator.Send(new GetCustomerCasesByIdentificationNumberAndStatusQuery
        {
            IdentificationNumber = identificationNumber,
            Status = status
        });

        return HandleApplicationEnterpriseResponse<GetCustomerCasesByIdentificationNumberAndStatusResponse>(response);
    }

    [SwaggerOperation(
        Summary = "Customer cases by reference number and case status.",
        Description = @"This request gets all customer cases given a particular status and the reference number linked to that case.")]
    [HttpGet]
    [Route("by/reference/{referenceNumber}/status/{status}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomerCasesByIdentificationNumberAndStatusResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetCustomerCasesByReferenceAndStatus(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem,
        string referenceNumber,
        string status)
    {
        GetCustomerCasesByReferenceNumberAndStatusResponse response = await Mediator.Send(new GetCustomerCasesByReferenceNumberAndStatusQuery
        {
            ReferenceNumber = referenceNumber,
            Status = status
        });

        return HandleApplicationEnterpriseResponse<GetCustomerCasesByReferenceNumberAndStatusResponse>(response);
    }

    [SwaggerOperation(
        Summary = "Create a shell case.",
        Description = @"This request creates a shell case with the only details being the id and reference number of the case.")]
    [HttpPost]
    [Route("create/shell")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateShellCaseCommandResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateShellCase(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem)
    {
        CreateShellCaseCommandResponse response = await Mediator.Send(new CreateShellCaseCommand() { SourceChannel = sourceSystem } );
        return HandleApplicationEnterpriseResponse<CreateShellCaseCommandResponse>(response);
    }

    [SwaggerOperation(
        Summary = "Create a case.",
        Description = @"This request creates a case using the customer identification provided.")]
    [HttpPost]
    [Route("create")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateOMCaseCommandResponse))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BaseFluentValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateOMCase(
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XSourceSystem)] CaseChannel sourceSystem,
        [Required, FromHeader(Name = CaseManagementConstants.HttpHeaders.XCustomerId)] string customerIdentificationNumber)
    {
        CreateOMCaseCommandResponse response = await Mediator.Send(new CreateOMCaseCommand() { SourceChannel = sourceSystem, IdentificationNumber = customerIdentificationNumber });
        return HandleApplicationEnterpriseResponse<CreateOMCaseCommandResponse>(response);
    }
}
