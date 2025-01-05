namespace K8s.SampleApi;

internal class GeneralSettings
{
    public required bool UseRedis { get; init; }

    public required string RedisConnectionString { get; init; }

    public required bool UseConsul { get; init; }

    public required string ConsulAddress { get; init; }

    public required string ConsulSelfServiceScheme { get; init; }

    public required string ConsulSelfServiceAddress { get; init; }

    public required int ConsulSelfServicePort { get; init; }
}
