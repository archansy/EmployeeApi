using System.Net;
using System.Net.Http.Json;
using EmployeeService.Application.DTOs;
using EmployeeService.IntegrationTests.Fixtures;
using FluentAssertions;

namespace EmployeeService.IntegrationTests.Employees;

public sealed class EmployeesEndpointsTests : IClassFixture<PostgreSqlContainerFixture>
{
    private readonly ApiWebApplicationFactory _factory;

    public EmployeesEndpointsTests(PostgreSqlContainerFixture fixture)
    {
        _factory = new ApiWebApplicationFactory(fixture.ConnectionString);
    }

    [Fact]
    public async Task Post_Then_GetById_Should_Return_Created_Employee()
    {
        using var client = _factory.CreateClient();

        var createRequest = new CreateEmployeeRequest
        {
            EmployeeNumber = "EMP-1001",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Department = "Engineering",
            JobTitle = "Senior Engineer",
            DateOfJoining = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3))
        };

        var postResponse = await client.PostAsJsonAsync("/api/v1/employees", createRequest);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<EmployeeDto>();
        created.Should().NotBeNull();

        var getResponse = await client.GetAsync($"/api/v1/employees/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
