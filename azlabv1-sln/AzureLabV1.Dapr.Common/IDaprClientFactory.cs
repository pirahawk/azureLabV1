namespace AzureLabV1.Dapr.Common
{
    public interface IDaprClientFactory
    {
        Task<HttpResponseMessage> CreateAppInvocationRequest<TRequestData>(HttpMethod httpMethod, string methodName, TRequestData data);
    }
}