using StackExchange.Redis;

namespace K8s.SampleApi;

internal class RedisKeepAliveBackgroundService(
    GeneralSettings generalSettings,
    AppInstanceIdProvider appInstanceIdProvider,
    RedisActiveInstancesContainer redisActiveInstancesContainer) : BackgroundService
{
    private static readonly TimeSpan KeepAliveUpdateTime = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan KeepAliveExpiryTime = TimeSpan.FromSeconds(10);

    private static readonly RedisValue ActiveInstancesPattern = new("ActiveInstance:SampleApi:*");

    private readonly GeneralSettings _generalSettings = generalSettings;
    private readonly AppInstanceIdProvider _appInstanceIdProvider = appInstanceIdProvider;
    private readonly RedisActiveInstancesContainer _redisActiveInstancesContainer = redisActiveInstancesContainer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_generalSettings.RedisConnectionString);
        var server = connectionMultiplexer.GetServer(_generalSettings.RedisConnectionString);
        var database = connectionMultiplexer.GetDatabase();

        var periodicTimer = new PeriodicTimer(KeepAliveUpdateTime);

        while (!stoppingToken.IsCancellationRequested)
        {
            await SendMyKeepAlive(database);

            await UpdateCurrentActiveInstances(server, database);

            await periodicTimer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task SendMyKeepAlive(IDatabase database)
    {
        var redisKey = new RedisKey($"ActiveInstance:SampleApi:{_appInstanceIdProvider.InstanceId}");
        var redisValue = new RedisValue(_appInstanceIdProvider.InstanceId);

        await database.StringSetAsync(redisKey, redisValue, KeepAliveExpiryTime, When.Always);
    }

    private async Task UpdateCurrentActiveInstances(IServer server, IDatabase database)
    {
        var activeInstanceKeys = await server
            .KeysAsync(pattern: ActiveInstancesPattern)
            .ToArrayAsync(CancellationToken.None);

        var activeInstanceValues = await database.StringGetAsync(activeInstanceKeys);

        var activeInstances = activeInstanceValues.Select(v => v.ToString());

        _redisActiveInstancesContainer.Update(activeInstances);
    }
}
