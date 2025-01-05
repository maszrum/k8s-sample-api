using System.Text;
using System.Text.Json;
using Consul;
using Consul.AspNetCore;
using K8s.SampleApi;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

var appInstanceId = Guid.NewGuid();

var builder = WebApplication.CreateBuilder(args);

var generalSettings = builder.Configuration
    .GetSection(nameof(GeneralSettings))
    .Get<GeneralSettings>()!;

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var dataProtectionBuilder = builder.Services
    .AddDataProtection()
    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });

if (generalSettings.UseRedis)
{
    dataProtectionBuilder.PersistKeysToStackExchangeRedis(
        connectionMultiplexer: ConnectionMultiplexer.Connect(generalSettings.RedisConnectionString),
        key: new RedisKey("DataProtectionKeys"));

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = generalSettings.RedisConnectionString;
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "K8s.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

if (generalSettings.UseConsul)
{
    builder.Services
        .AddConsul(configuration =>
        {
            configuration.Address = new Uri(generalSettings.ConsulAddress);
        })
        .AddConsulServiceRegistration(registration =>
        {
            registration.Name = Constants.ConsulServiceName;
            registration.ID = appInstanceId.ToString("D");
            registration.Address = generalSettings.ConsulSelfServiceAddress;
            registration.Port = generalSettings.ConsulSelfServicePort;

            var healthUrl = new StringBuilder()
                .Append(generalSettings.ConsulSelfServiceScheme)
                .Append("://")
                .Append(generalSettings.ConsulSelfServiceAddress)
                .Append(':')
                .Append(generalSettings.ConsulSelfServicePort)
                .Append("/health")
                .ToString();

            registration.Check = new AgentCheckRegistration
            {
                Name = Constants.ConsulHealthcheckName,
                Method = "GET",
                HTTP = healthUrl,
                Interval = TimeSpan.FromSeconds(10)
            };
        });

    builder.Services.AddSingleton<IActiveInstancesResolver, ConsulActiveInstancesResolver>();

    builder.Services.AddSingleton<ISynchronizationStrategy, ConsulDistributedLock>();
}
else
{
    builder.Services.AddSingleton<IActiveInstancesResolver>(new SingleNodeInstanceResolver(appInstanceId));

    builder.Services.AddSingleton<ISynchronizationStrategy, LocalLock>();
}

var app = builder.Build();

app.UseSession();

app.MapGet(
    "/counter",
    async (
        HttpContext httpContext,
        [FromServices] IActiveInstancesResolver activeInstancesResolver,
        [FromServices] ISynchronizationStrategy synchronizationStrategy) =>
    {
        var newCounter = 0;

        await synchronizationStrategy.ExecuteLocked(
            synchronizationKey: $"counter-{httpContext.Session.Id}",
            action: () =>
            {
                var currentCounter = httpContext.Session.GetInt32(Constants.UserCounterCacheKey) ?? 0;
                newCounter = currentCounter + 1;
                httpContext.Session.SetInt32(Constants.UserCounterCacheKey, newCounter);
            },
            cancellationToken: CancellationToken.None);
        var activeInstanceIds = await activeInstancesResolver.GetActiveInstances();

        return new CounterResponse(
            appInstanceId,
            httpContext.Session.Id,
            activeInstanceIds,
            newCounter);
    });

app.MapGet("/health", () => Results.Ok());

app.Run();
