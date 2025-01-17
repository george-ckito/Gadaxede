using Gadaxede.Interfaces;
using Gadaxede.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gadaxede.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SensorController : Controller
    {
        private readonly ISensorRepository _sensorRepository;
        public SensorController(ISensorRepository ISensorRepository)
        {
            _sensorRepository = ISensorRepository;
        }

        [HttpGet]
        public IActionResult GetSensors()
        {
            var sensors = _sensorRepository.GetSensors();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(sensors);
        }

        [HttpGet("{id}")]
        public IActionResult GetSensor(int id)
        {
            var sensor = _sensorRepository.GetSensor(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(sensor);
        }
    }
}
