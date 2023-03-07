using Dapr;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AzureLabV1.Dapr.SampleWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost("process",Name = "processOrders")]
        [Topic("orderpubsub", "orders")]
        public IActionResult ProcessOrder(Order order)
        {
            _logger.LogInformation("Order Recived data: " + order.OrderId);
            return Ok(order);
        }
    }

    public record Order([property: JsonPropertyName("orderId")] int OrderId);
}