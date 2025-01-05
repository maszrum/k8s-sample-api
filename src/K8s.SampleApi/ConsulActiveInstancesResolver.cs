using System.Collections.Immutable;
using Consul;

namespace K8s.SampleApi;

internal class ConsulActiveInstancesResolver(IConsulClient consulClient) : IActiveInstancesResolver
{
    private readonly IConsulClient _consulClient = consulClient;

    public async Task<ImmutableArray<string>> GetActiveInstances()
    {
        var serviceEntries = await _consulClient.Health.Service(Constants.ConsulServiceName);

        var activeInstanceIds = serviceEntries.Response
            .SelectMany(entry => entry.Checks)
            .Where(check =>
                check.Status.Equals(HealthStatus.Passing) &&
                check.Name.Equals(Constants.ConsulHealthcheckName, StringComparison.Ordinal))
            .Select(check => check.ServiceID)
            .ToImmutableArray();

        return activeInstanceIds;
    }
}
