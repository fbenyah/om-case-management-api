using NBomber.CSharp;
using NBomber.Contracts;

namespace om.servicing.casemanagement.tests.performance.Api.Controllers.V1;

public class CaseControllerPerformanceTests
{
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("PERF_BASE_URL") ?? "https://localhost:5001";

    // Performance tests should be executed separately; the test is skipped by default.
    [Fact(Skip = "Performance test — run manually or via a dedicated CI job. Remove Skip to execute.")]
    public void GetCasesByIdentification_SteadyLoadScenario()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        var step = Step.Create("get_cases_by_identification", async context =>
        {
            var id = "1234567890"; // choose a realistic test id or parameterize via env vars
            using var res = await client.GetAsync($"{_baseUrl}/api/casemanagement/v1/Case/by/identification/{id}");
            return res.IsSuccessStatusCode
                ? Response.Ok(statusCode: (int)res.StatusCode)
                : Response.Fail(statusCode: (int)res.StatusCode, error: $"status={(int)res.StatusCode}");
        });

        var scenario = Scenario.Create("get_cases_scenario", step)
            // Start with a modest load: 10 virtual users for 30 seconds (adjust for your environment)
            .WithLoadSimulations(Simulation.KeepConstant(10, TimeSpan.FromSeconds(30)));

        // Run NBomber — returns 0 on success, non-zero on failure
        int exitCode = NBomberRunner.RegisterScenarios(scenario).Run().GetAwaiter().GetResult();

        // Basic smoke assertion: runner exit code 0 (NBomber-level success)
        Assert.Equal(0, exitCode);
    }
}
