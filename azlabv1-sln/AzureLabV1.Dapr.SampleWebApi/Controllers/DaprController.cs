using Dapr;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AzureLabV1.Dapr.SampleWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DaprController : ControllerBase
    {
        private readonly ILogger<DaprController> _logger;

        public DaprController(ILogger<DaprController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "orders")]
        [Topic("orderpubsub", "orders")]
        public IActionResult DaprTest(Order order)
        {
            _logger.LogInformation("Order Recived data: " + order.OrderId);
            return Ok(order);
        }
    }

    public record Order([property: JsonPropertyName("orderId")] int OrderId);
}