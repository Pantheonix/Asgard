using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

var jwtConfig = new JwtConfig();
builder.Configuration.Bind(nameof(jwtConfig), jwtConfig);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddFastEndpoints(options =>
    {
        options.DisableAutoDiscovery = true;
        options.Assemblies = new[]
        {
            typeof(IApiMarker).Assembly,
            typeof(IApplicationMarker).Assembly
        };
    })
    .AddSingleton(jwtConfig)
    .AddJWTBearerAuth(jwtConfig.SecretKey)
    .AddAutoMapper(typeof(IApiMarker), typeof(IApplicationMarker))
    .AddSwaggerDoc(settings =>
    {
        settings.Title = "Quetzalcoatl Auth API";
        settings.Version = "v1";
    });

var app = builder.Build();

app.UseDefaultExceptionHandler()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(config =>
    {
        config.Endpoints.RoutePrefix = "api";
    })
    .UseSwaggerGen();

app.Run();
