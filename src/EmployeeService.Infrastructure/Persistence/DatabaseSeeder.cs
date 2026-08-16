using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Enums;
using EmployeeService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmployeeService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAdminUserAsync(ApplicationDbContext dbContext, IConfiguration configuration, ILogger logger, CancellationToken cancellationToken)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        var adminEmail = "admin@employeehub.com";
        var existing = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == adminEmail, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var adminPassword = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD")
            ?? configuration["Seed:AdminPassword"]
            ?? "Admin@12345";

        var now = DateTime.UtcNow;
        var hasher = new PasswordHasher();

        var admin = new User(
            Guid.NewGuid(),
            "EmployeeHub Admin",
            adminEmail,
            hasher.HashPassword(adminPassword),
            UserRole.Admin,
            UserStatus.Active,
            now,
            now);

        await dbContext.Users.AddAsync(admin, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded default admin user {Email}", adminEmail);
    }
}
