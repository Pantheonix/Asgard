using Api.Features.Auth.Register;

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
    options.Assemblies = new[] { typeof(RegisterUserEndpoint).Assembly };
});

var jwtConfig = new JwtConfig();
builder.Configuration.Bind(nameof(jwtConfig), jwtConfig);

builder.Services.AddSingleton(jwtConfig);
builder.Services.AddJWTBearerAuth(jwtConfig.SecretKey);

builder.Services.AddAutoMapper(typeof(IApiMarker));

var app = builder.Build();

app.UseDefaultExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();
