using EmployeeService.Application.DTOs;
using EmployeeService.Domain.Entities;
using Mapster;

namespace EmployeeService.Application.Mappings;

public static class EmployeeMappingConfig
{
    public static void RegisterMappings(TypeAdapterConfig config)
    {
        config.NewConfig<Employee, EmployeeDto>()
            .Map(dest => dest.ConcurrencyToken, src => Convert.ToBase64String(src.RowVersion));
    }
}
