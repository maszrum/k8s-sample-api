namespace K8s.SampleApi;

internal record CounterResponse(
    Guid AppInstanceId,
    string SessionId,
    IReadOnlySet<string> ActiveInstances,
    int Counter);
