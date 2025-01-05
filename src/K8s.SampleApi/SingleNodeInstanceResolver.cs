using System.Collections.Immutable;

namespace K8s.SampleApi;

internal class SingleNodeInstanceResolver(Guid singleNodeInstanceId) : IActiveInstancesResolver
{
    private readonly string _singleNodeInstanceId = singleNodeInstanceId.ToString("D");

    public Task<ImmutableArray<string>> GetActiveInstances() =>
        Task.FromResult(ImmutableArray.Create(_singleNodeInstanceId));
}
