using Dapr.Actors;

namespace AzureLabV1.Dapr.Common
{
    public interface IOrderActor: IActor
    {
        Task<string> SaveStateAsync(OrderDataState orderData);
        Task<OrderDataState?> GetOrderStateAsync();
    }
}