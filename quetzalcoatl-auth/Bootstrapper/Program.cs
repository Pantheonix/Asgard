Log.Logger = new LoggerConfiguration().MinimumLevel
    .Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    var jwtConfig = new JwtConfig();
    builder.Configuration.Bind(nameof(jwtConfig), jwtConfig);

    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services)
    );

    builder.Services
        .AddDbContext<AppDbContext>(
            options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        )
        .AddScoped<IPictureRepository, PictureRepository>()
        .AddIdentity<ApplicationUser, ApplicationRole>(identity =>
        {
            identity.User.RequireUniqueEmail = true;
        })
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

    app.UseSerilogRequestLogging()
        .UseDefaultExceptionHandler()
        .UseAuthentication()
        .UseAuthorization()
        .UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
        })
        .UseSwaggerGen();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
