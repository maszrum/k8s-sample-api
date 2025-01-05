namespace K8s.SampleApi;

internal class AppInstanceIdProvider
{
    public AppInstanceIdProvider()
    {
        InstanceGuid = Guid.NewGuid();
        InstanceId = InstanceGuid.ToString("D");
    }

    public Guid InstanceGuid { get; }

    public string InstanceId { get; }
}
