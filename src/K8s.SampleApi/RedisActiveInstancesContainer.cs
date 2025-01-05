using System.Collections.Immutable;

namespace K8s.SampleApi;

internal class RedisActiveInstancesContainer : IActiveInstancesResolver
{
    private ImmutableHashSet<string> _activeInstances = ImmutableHashSet<string>.Empty;

    public void Update(IEnumerable<string> activeInstances)
    {
        var activeInstancesImmutable = ImmutableHashSet.CreateRange(activeInstances);

        if (activeInstancesImmutable.Count != _activeInstances.Count ||
            !activeInstancesImmutable.SetEquals(_activeInstances))
        {
            _activeInstances = activeInstancesImmutable;
        }
    }

    public ImmutableHashSet<string> GetActiveInstances() => _activeInstances;
}
