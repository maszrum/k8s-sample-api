using System.Collections.Concurrent;

namespace K8s.SampleApi;

internal class LocalLock : ISynchronizationStrategy
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new(StringComparer.Ordinal);

    public async Task ExecuteLocked(string synchronizationKey, Action action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(synchronizationKey);
        ArgumentNullException.ThrowIfNull(action);

        var semaphore = _semaphores.GetOrAdd(synchronizationKey, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);

        try
        {
            action();
        }
        finally
        {
            semaphore.Release();

            if (semaphore.CurrentCount == 1)
            {
                _semaphores.TryRemove(synchronizationKey, out _);
            }
        }
    }
}
