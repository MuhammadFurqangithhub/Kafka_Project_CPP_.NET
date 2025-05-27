using KafkaWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace KafkaWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly KafkaProducer _producerService;

        public MessagesController(KafkaProducer producerService)
        {
            _producerService = producerService;
        }


        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] object jsonMessage)
        {
            if (jsonMessage == null)
                return BadRequest("Message cannot be null.");

            var jsonString = jsonMessage.ToString();
            await _producerService.SendMessageAsync(jsonString);

            return Ok("JSON message sent to Kafka.");
        }
    }
}
