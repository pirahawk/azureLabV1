using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        [HttpPost(Name = "OrderPub")]
        public async Task<IActionResult> PostOrder([FromBody]Order orderToSend)
        {
            // Publish an event/message using Dapr PubSub
            await this.daprClient.PublishEventAsync("orderpubsub", "orders", orderToSend);
            _logger.LogInformation("Published data: " + orderToSend.OrderId);
            return StatusCode((int)HttpStatusCode.Accepted, orderToSend);
        }
    }

    public record Order([property: JsonPropertyName("orderId")] int OrderId);
}