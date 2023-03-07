using Dapr.Client;

namespace AzureLabV1.Dapr.Common
{
    public class DaprClientFactory : IDaprClientFactory
    {
        private readonly string targetAppID;

        public DaprClientFactory(string targetAppID)
        {
            this.targetAppID = targetAppID;
        }

        public async Task<HttpResponseMessage> CreateAppInvocationRequest<TRequestData>(HttpMethod httpMethod, string methodName, TRequestData data)
        {
            var daprClient = new DaprClientBuilder().Build();
            var requestMessage = daprClient.CreateInvokeMethodRequest(httpMethod, targetAppID, methodName, data);
            var response = await daprClient.InvokeMethodAsync<HttpResponseMessage>(requestMessage);
            return response;
        }
    }
}