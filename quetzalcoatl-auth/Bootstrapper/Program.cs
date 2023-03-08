var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddFastEndpoints(options =>
{
    options.DisableAutoDiscovery = true;
    options.Assemblies = new[] { typeof(RegisterEndpoint).Assembly };
});

var jwtConfig = new JwtConfig();
builder.Configuration.Bind(nameof(jwtConfig), jwtConfig);

builder.Services.AddSingleton(jwtConfig);
builder.Services.AddJWTBearerAuth(jwtConfig.SecretKey);

var app = builder.Build();

app.UseDefaultExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();
