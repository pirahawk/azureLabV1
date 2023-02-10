using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AzureLabV1.Dapr.SampleWebApi.ClientApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DaprClientController : ControllerBase
    {
        
        private readonly ILogger<DaprClientController> _logger;
        private readonly DaprClient daprClient;

        public DaprClientController(ILogger<DaprClientController> logger, DaprClient daprClient)
        {
            _logger = logger;
            this.daprClient = daprClient;
        }

        [HttpGet(Name = "OrderPub")]
        public async Task<Order> PostOrder()
        {
            var order = new Order(1);

            // Publish an event/message using Dapr PubSub
            await this.daprClient.PublishEventAsync("orderpubsub", "orders", order);
            _logger.LogInformation("Published data: " + order);
            return order;
        }
    }

    public record Order([property: JsonPropertyName("orderId")] int OrderId);
}