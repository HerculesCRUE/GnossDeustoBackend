using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_CARGA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestRabbitMqController : ControllerBase
    {
        private readonly IRabbitMQService amqpService;

        public TestRabbitMqController(IRabbitMQService amqpService)
        {
            this.amqpService = amqpService ?? throw new ArgumentNullException(nameof(amqpService));
        }

        [HttpPost("")]
        public IActionResult PublishMessage([FromBody] object message)
        {
            amqpService.PublishMessage(message);
            return Ok();
        }
    }
}