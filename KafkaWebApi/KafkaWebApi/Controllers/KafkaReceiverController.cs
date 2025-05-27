using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace KafkaWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KafkaReceiverController : ControllerBase
    {
        private readonly ILogger<KafkaReceiverController> _logger;
        private readonly MessageStore _messageStore;

        public KafkaReceiverController(ILogger<KafkaReceiverController> logger, MessageStore messageStore)
        {
            _logger = logger;
            _messageStore = messageStore;
        }

        [HttpGet("messages")]
        public IActionResult GetMessages()
        {
            var messages = _messageStore.GetMessages();

            var formattedMessages = messages.Select(message =>
            {
                // Parse the message string to extract the "message" field
                var json = JObject.Parse(message); // Parse JSON
                var actualMessage = json["message"]?.ToString(); // Extract the "message" value

                return new { message = actualMessage };
            });

            return Ok(formattedMessages);
        }
    }
}
