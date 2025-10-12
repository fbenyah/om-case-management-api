using Microsoft.AspNetCore.Mvc;
using om.servicing.casemanagement.api.Controllers;
using om.servicing.casemanagement.domain.Exceptions.Client;
using om.servicing.casemanagement.domain.Responses.Shared;
using OM.RequestFramework.Core.Exceptions;

namespace om.servicing.casemanagement.tests.Api.Controllers;

public class BaseControllerTests
{
    [Fact]
    public void HandleApplicationEnterpriseResponse_ReturnsOk_WhenSuccessIsTrue()
    {
        var controller = new TestController();
        var response = new TestResponse { };

        var result = controller.TestHandleApplicationEnterpriseResponse(response);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public void HandleApplicationEnterpriseResponse_ReturnsBadRequest_WhenSuccessIsFalse_AndNoCustomException()
    {
        var controller = new TestController();
        var response = new TestResponse { };
        response.SetOrUpdateErrorMessage("Something failed, so we expect a bad response");

        var result = controller.TestHandleApplicationEnterpriseResponse(response);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(response, badRequestResult.Value);
    }

    [Fact]
    public void HandleApplicationEnterpriseResponse_ReturnsConflict_WhenConflictExceptionPresent()
    {
        var controller = new TestController();
        var response = new TestResponse { };
        response.SetOrUpdateCustomException(new ConflictException("conflict"));

        var result = controller.TestHandleApplicationEnterpriseResponse(response);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, conflictResult.StatusCode);
        Assert.Equal(response, conflictResult.Value);
    }

    [Fact]
    public void HandleApplicationEnterpriseResponse_ReturnsNoContent_WhenNotFoundExceptionPresent()
    {
        var controller = new TestController();
        var response = new TestResponse { };
        response.SetOrUpdateCustomException(new NotFoundException("not found"));

        var result = controller.TestHandleApplicationEnterpriseResponse(response);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public void HandleApplicationEnterpriseResponse_ReturnsTooManyRequests_WhenTooManyRequestsExceptionPresent()
    {
        var controller = new TestController();
        var response = new TestResponse { };
        response.SetOrUpdateCustomException(new TooManyRequestsException("too many"));

        var result = controller.TestHandleApplicationEnterpriseResponse(response);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, objectResult.StatusCode);
        Assert.Equal(response, objectResult.Value);
    }
}

public class TestResponse : BaseFluentValidationError
{
    public TestResponse() { }
}

public class TestController : BaseController
{
    // Expose the protected method for testing
    public IActionResult TestHandleApplicationEnterpriseResponse(TestResponse response)
        => HandleApplicationEnterpriseResponse(response);
}
