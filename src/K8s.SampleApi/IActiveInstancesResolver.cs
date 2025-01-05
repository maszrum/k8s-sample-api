using System.Collections.Immutable;

namespace K8s.SampleApi;

internal interface IActiveInstancesResolver
{
    Task<ImmutableArray<string>> GetActiveInstances();
}
