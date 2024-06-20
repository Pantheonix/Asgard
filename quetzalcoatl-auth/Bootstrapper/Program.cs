using Domain.Consts;
using DotNetEnv;
using DotNetEnv.Configuration;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.IsEnvironment(SystemConsts.TestingEnvironment))
    {
        _ = builder.Configuration.AddDotNetEnv(".env.template", LoadOptions.TraversePath()).Build();
    }

    builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
    builder.Services.Configure<AdminConfig>(builder.Configuration.GetSection(nameof(AdminConfig)));

    var jwtConfig = builder.Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>();
    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig!.SecretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
    var dsnConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (!builder.Environment.IsEnvironment(SystemConsts.TestingEnvironment))
    {
        builder.Services.AddHealthChecks().AddSqlServer(dsnConnectionString!);
    }

    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services)
    );

    builder
        .Services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (!builder.Environment.IsEnvironment(SystemConsts.TestingEnvironment))
            {
                options.UseSqlServer(dsnConnectionString);
            }

            options.UseTriggers(triggerOptions =>
                triggerOptions.AddTrigger<DeleteStaleRefreshTokens>()
            );
        })
        .AddScoped<IPictureRepository, PictureRepository>()
        .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
        .AddIdentity<ApplicationUser, IdentityRole<Guid>>(identity =>
        {
            identity.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.ApplyMigrations<ApplicationDbContext>();
    }

    var corsOrigins = builder.Configuration.GetSection("AllowedOrigins").Value?.Split(';');
    Log.Information("Allowed origins: {CorsOrigins}", corsOrigins);
    builder
        .Services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .WithOrigins(
                        corsOrigins ?? ["http://localhost:10000", "https://pantheonix.live"]
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        })
        .AddSingleton(tokenValidationParameters)
        .AddAuthenticationJwtBearer(opt =>
        {
            opt.SigningKey = jwtConfig.SecretKey;
        })
        .AddAuthorization()
        .AddAutoMapper(typeof(IApiMarker), typeof(IApplicationMarker))
        .AddFastEndpoints(options =>
        {
            options.DisableAutoDiscovery = true;
            options.Assemblies = [typeof(IApiMarker).Assembly, typeof(IApplicationMarker).Assembly];
        });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        await app.UseSeedData();
    }

    if (!app.Environment.IsEnvironment(SystemConsts.TestingEnvironment))
    {
        app.MapHealthChecks(
                "/_health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                }
            )
            .RequireHost("*:5210");
    }

    app.UseSerilogRequestLogging()
        .UseDefaultExceptionHandler()
        .UseCors()
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
