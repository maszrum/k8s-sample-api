using System.Collections.Immutable;

namespace K8s.SampleApi;

internal interface IActiveInstancesResolver
{
    ImmutableHashSet<string> GetActiveInstances();
}
