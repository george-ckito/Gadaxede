using Gadaxede.Models;
using Gadaxede.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gadaxede.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISensorRepository _sensorRepository;

        public UserController(IUserRepository userRepository, ISensorRepository sensorRepository)
        {
            _userRepository = userRepository;
            _sensorRepository = sensorRepository;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users);
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            _userRepository.RegisterUser(user);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(user);
        }

        [HttpPut("edit")]
        public IActionResult EditUser([FromBody] User user)
        {
            _userRepository.EditUser(user);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _userRepository.GetUser(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            _userRepository.DeleteUser(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpPatch("{id}/add-sensor")]
        public IActionResult AddSensor(int id, [FromBody] ICollection<string> sensors)
        {
            var user = _userRepository.GetUser(id);
            foreach (var s in sensors)
            {
                _userRepository.AddSensor(user, _sensorRepository.GetSensor(s));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}
