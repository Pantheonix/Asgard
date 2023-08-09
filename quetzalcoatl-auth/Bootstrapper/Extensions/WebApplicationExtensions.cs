namespace Bootstrapper.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseSeedData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var roles = Enum.GetValues<ApplicationRoles>();
        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role.ToString());
            if (roleExists)
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role.ToString()));
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create role {role}");
            }
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var adminConfig = services.GetRequiredService<IOptions<AdminConfig>>();

        var adminUser = new ApplicationUser
        {
            UserName = adminConfig.Value.UserName,
            Email = adminConfig.Value.Email
        };

        var adminUserExists = await userManager.FindByEmailAsync(adminConfig.Value.Email);

        if (adminUserExists is null)
        {
            var resultCreateUser = await userManager.CreateAsync(
                adminUser,
                adminConfig.Value.Password
            );
            var resultAddRoles = await userManager.AddToRolesAsync(
                adminUser,
                new[] { ApplicationRoles.Proposer.ToString(), ApplicationRoles.Admin.ToString() }
            );

            if (!resultCreateUser.Succeeded || !resultAddRoles.Succeeded)
            {
                throw new Exception("Failed to create admin user");
            }
        }
    }
}
