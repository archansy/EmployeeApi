using FluentAssertions;
using NetArchTest.Rules;

namespace EmployeeService.ArchitectureTests;

public sealed class DependencyRulesTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Application_Infrastructure_Or_Api()
    {
        var result = Types.InAssembly(typeof(EmployeeService.Domain.Entities.Employee).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("EmployeeService.Application", "EmployeeService.Infrastructure", "EmployeeService.Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Only_Depend_On_Domain_From_Core_Layers()
    {
        var result = Types.InAssembly(typeof(EmployeeService.Application.DependencyInjection.ApplicationServiceRegistration).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("EmployeeService.Infrastructure", "EmployeeService.Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(typeof(EmployeeService.Infrastructure.DependencyInjection.InfrastructureServiceRegistration).Assembly)
            .ShouldNot()
            .HaveDependencyOn("EmployeeService.Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
