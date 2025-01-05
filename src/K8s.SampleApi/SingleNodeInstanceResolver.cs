using System.Collections.Immutable;

namespace K8s.SampleApi;

internal class SingleNodeInstanceResolver(AppInstanceIdProvider appInstanceIdProvider) : IActiveInstancesResolver
{
    private readonly ImmutableHashSet<string> _appInstanceIdProvider = [appInstanceIdProvider.InstanceId];

    public ImmutableHashSet<string> GetActiveInstances() => _appInstanceIdProvider;
}
