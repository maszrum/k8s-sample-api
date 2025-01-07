namespace K8s.SampleApi;

internal class ApplicationHealth
{
    public bool IsHealthy { get; private set; } = true;

    public void Set(bool isHealthy)
    {
        IsHealthy = isHealthy;
    }
}
