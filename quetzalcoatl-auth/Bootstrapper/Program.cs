Log.Logger = new LoggerConfiguration().MinimumLevel
    .Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    var jwtConfig = new JwtConfig();
    builder.Configuration.Bind(nameof(jwtConfig), jwtConfig);

    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };

    var adminConfig = new AdminConfig();
    builder.Configuration.Bind(nameof(adminConfig), adminConfig);

    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services)
    );

    builder.Services
        .AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseTriggers(triggerOptions => triggerOptions.AddTrigger<DeleteStaleRefreshTokens>());
            }
        )
        .AddScoped<IPictureRepository, PictureRepository>()
        .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
        .AddIdentity<ApplicationUser, IdentityRole<Guid>>(identity =>
        {
            identity.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();

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
        .AddSingleton(adminConfig)
        .AddSingleton(tokenValidationParameters)
        .AddJWTBearerAuth(jwtConfig.SecretKey)
        .AddAutoMapper(typeof(IApiMarker), typeof(IApplicationMarker))
        .AddSwaggerDoc(settings =>
        {
            settings.Title = "Quetzalcoatl Auth API";
            settings.Version = "v1";
        });

    var app = builder.Build();

    await app.UseSeedData();

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
