using System;
using System.Linq;
using System.Text;
using Asgard.Hermes;
using EnkiProblems.MongoDB;
using EnkiProblems.MultiTenancy;
using EnkiProblems.Problems;
using EnkiProblems.Problems.Tests;
using Grpc.Net.Client;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace EnkiProblems;

[DependsOn(
    typeof(EnkiProblemsHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(EnkiProblemsApplicationModule),
    typeof(EnkiProblemsMongoDbModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class EnkiProblemsHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureLogger(context, configuration);
        ConfigureHttpClient(context, configuration);
        ConfigureDapr(context, configuration);
        ConfigureHermesTestsGrpcClient(context, configuration);
        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureCache(configuration);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
    }

    private void ConfigureLogger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddLogging(config =>
        {
            config.AddConfiguration(configuration.GetSection("Logging"));
            config.AddConsole();
            config.AddDebug();
        });
    }

    private void ConfigureHttpClient(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        context.Services.AddHttpClient();
    }

    private void ConfigureDapr(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var hermesAppId = configuration["Dapr:HermesAppId"];
        var address = configuration["Dapr:GrpcEndpoint"];

        context
            .Services.AddSingleton<DaprMetadata>(_ =>
                new() { HermesContext = new() { { "dapr-app-id", hermesAppId! } } }
            )
            .AddDaprClient(options =>
            {
                options.UseJsonSerializationOptions(new() { PropertyNameCaseInsensitive = true });
                options.UseGrpcEndpoint(address);
            });
    }

    private void ConfigureHermesTestsGrpcClient(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        var address = configuration["Dapr:GrpcEndpoint"];
        var channel = GrpcChannel.ForAddress(address!);

        context.Services.AddSingleton<HermesTestsService.HermesTestsServiceClient>(_ =>
            new(channel)
        );
        context.Services.AddScoped<ITestService, HermesTestsGrpcService>();
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "EnkiProblems:";
        });
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateTestDto));
            options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(UpdateTestDto));
            options.ConventionalControllers.Create(
                typeof(EnkiProblemsApplicationModule).Assembly,
                opts =>
                {
                    opts.RootPath = "enki";
                }
            );
        });
    }

    private void ConfigureAuthentication(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        context
            .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["AuthServer:SecurityKey"]!)
                    ),
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }

    private static void ConfigureSwaggerServices(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo { Title = "EnkiProblems API", Version = "v1" }
            );
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                }
            );
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );
        });
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment
    )
    {
        var dataProtectionBuilder = context
            .Services.AddDataProtection()
            .SetApplicationName("EnkiProblems");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(
                redis,
                "EnkiProblems-Protection-Keys"
            );
        }
    }

    private void ConfigureDistributedLocking(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["AllowedOrigins"]
                            ?.Split(";", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCloudEvents();
        app.UseCors();
        app.UseAuthentication();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "EnkiProblems API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("EnkiProblems");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints(options =>
        {
            options.MapSubscribeHandler();
        });
    }
}
