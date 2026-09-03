FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/EmployeeService.Api/EmployeeService.Api.csproj", "src/EmployeeService.Api/"]
COPY ["src/EmployeeService.Application/EmployeeService.Application.csproj", "src/EmployeeService.Application/"]
COPY ["src/EmployeeService.Domain/EmployeeService.Domain.csproj", "src/EmployeeService.Domain/"]
COPY ["src/EmployeeService.Infrastructure/EmployeeService.Infrastructure.csproj", "src/EmployeeService.Infrastructure/"]
RUN dotnet restore "src/EmployeeService.Api/EmployeeService.Api.csproj"

COPY . .
RUN dotnet publish "src/EmployeeService.Api/EmployeeService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN adduser --disabled-password --home /app --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "EmployeeService.Api.dll"]
