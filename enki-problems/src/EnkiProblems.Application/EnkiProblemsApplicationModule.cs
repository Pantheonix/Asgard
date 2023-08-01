using System;
using Asgard.Hermes;
using EnkiProblems.Problems.Tests;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace EnkiProblems;

[DependsOn(
    typeof(EnkiProblemsDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(EnkiProblemsApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
)]
public class EnkiProblemsApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<EnkiProblemsApplicationModule>();
        });

        ConfigureDapr(context, configuration);
        ConfigureHermesTestsGrpcClient(context, configuration);
    }

    private void ConfigureDapr(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var hermesAppId = configuration["Dapr:HermesAppId"];
        context.Services.AddSingleton<DaprMetadata>(
            _ => new() { HermesContext = new() { { "dapr-app-id", hermesAppId! } } }
        );
    }

    private void ConfigureHermesTestsGrpcClient(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        var address = configuration["Dapr:HermesAddress"];
        var channel = GrpcChannel.ForAddress(address!);

        context.Services.AddSingleton<HermesTestsService.HermesTestsServiceClient>(
            _ => new(channel)
        );
        context.Services.AddScoped<ITestsService, HermesTestsGrpcService>();
    }
}
