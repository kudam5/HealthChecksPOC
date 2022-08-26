using HealthMonitorPOC.Web.Kafka;
using HealthMonitorPOC.Web.Settings;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitorPOC.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ProducersController: ControllerBase
    {
        private readonly IKafkaProducer _producer;

        public ProducersController(IKafkaProducer producer)
        {
            _producer = producer;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _producer.GetEvents().ConfigureAwait(false);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(PublishRequest model)
        {            
            var response = await _producer.PublishEvent(model).ConfigureAwait(false);

            return Ok(response);
        }     
    }
}
