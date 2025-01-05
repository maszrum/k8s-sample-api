namespace K8s.SampleApi;

internal interface ISynchronizationStrategy
{
    Task ExecuteLocked(string synchronizationKey, Action action, CancellationToken cancellationToken);
}
