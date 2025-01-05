using Consul;

namespace K8s.SampleApi;

internal class ConsulDistributedLock(IConsulClient consulClient) : ISynchronizationStrategy
{
    private readonly IConsulClient _consulClient = consulClient;

    public async Task ExecuteLocked(
        string synchronizationKey,
        Action action,
        CancellationToken cancellationToken = default)
    {
        var lockOptions = new LockOptions(synchronizationKey)
        {
            SessionTTL = TimeSpan.FromSeconds(15),
            LockTryOnce = true,
            LockWaitTime = TimeSpan.FromSeconds(5),
            LockRetryTime = TimeSpan.FromSeconds(1),
            MonitorRetries = 3,
            MonitorRetryTime = TimeSpan.FromSeconds(1)
        };

        var distributedLock = await _consulClient.AcquireLock(lockOptions, cancellationToken);

        try
        {
            if (!distributedLock.IsHeld)
            {
                throw new LockNotHeldException("Could not obtain the lock.");
            }

            action();
        }
        finally
        {
            await distributedLock.Release(cancellationToken);
        }
    }
}
