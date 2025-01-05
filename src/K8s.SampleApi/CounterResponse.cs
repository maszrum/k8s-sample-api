namespace K8s.SampleApi;

internal record CounterResponse(
    Guid AppInstanceId,
    string SessionId,
    IReadOnlyList<string> ActiveInstances,
    int Counter);
