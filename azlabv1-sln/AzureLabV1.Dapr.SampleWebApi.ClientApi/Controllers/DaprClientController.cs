using AzureLabV1.Dapr.Common;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureLabV1.Dapr.SampleWebApi.ClientApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DaprClientController : ControllerBase
    {
        private readonly ILogger<DaprClientController> _logger;
        private readonly DaprClient daprClient;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IActorProxyFactory actorProxyFactory;
        private readonly IDaprClientFactory daprClientFactory;

        public DaprClientController(ILogger<DaprClientController> logger,
            DaprClient daprClient,
            IHttpClientFactory httpClientFactory,
            IActorProxyFactory actorProxyFactory,
            IDaprClientFactory daprClientFactory)
        {
            _logger = logger;
            this.daprClient = daprClient;
            this.httpClientFactory = httpClientFactory;
            this.actorProxyFactory = actorProxyFactory;
            this.daprClientFactory = daprClientFactory;
        }

        [HttpPost("orderPublish", Name = "orderPublish")]
        public async Task<IActionResult> PostOrder([FromBody] Order orderToSend)
        {
            // Publish an event/message using Dapr PubSub
            await this.daprClient.PublishEventAsync("orderpubsub", "orders", orderToSend);
            _logger.LogInformation("Published data: " + orderToSend.OrderId);
            return StatusCode((int)HttpStatusCode.Accepted, orderToSend);
        }

        [HttpPost("orderInvoke", Name = "orderInvoke")]
        public async Task<IActionResult> PostOrderInvoke([FromBody] Order orderToSend)
        {
            //var client = this.httpClientFactory.CreateClient("daprServiceInvocation");

            //var orderJson = JsonSerializer.Serialize(orderToSend);
            //var content = new StringContent(orderJson, Encoding.UTF8, "application/json");
            //var response = await client.PostAsync($"order", content);

            var response = await daprClientFactory.CreateAppInvocationRequest(HttpMethod.Post, "order/process", orderToSend);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.Accepted, orderToSend);
        }

        [HttpPost("orderActorInvoke", Name = "orderActorInvoke")]
        public async Task<IActionResult> PostActorInvoke([FromBody] OrderDataState orderData)
        {
            var actorType = nameof(OrderActor);
            var actorId = new ActorId($"{orderData.Id}");
            var proxy = this.actorProxyFactory.CreateActorProxy<IOrderActor>(actorId, actorType);
            var response = await proxy.SaveStateAsync(orderData);
            return StatusCode((int)HttpStatusCode.Accepted, response);
        }
    }

    public record Order([property: JsonPropertyName("orderId")] int OrderId);
}