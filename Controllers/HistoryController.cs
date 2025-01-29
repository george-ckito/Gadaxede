using Gadaxede.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gadaxede.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HistoryController : Controller
    {
        private readonly IHistoryRepository _historyRepository;
        public HistoryController(IHistoryRepository IHistoryRepository)
        {
            _historyRepository = IHistoryRepository;
        }
        [HttpGet]
        public IActionResult GetHistory()
        {
            return Ok(_historyRepository.GetMeasurements());
        }
        [HttpGet("minute")]
        public IActionResult GetMinuteHistory()
        {
            var result = _historyRepository.GetMinuteMeasurements();
            var answer = new List<Dictionary<string, object>>();
            return Ok();
        }
        [HttpGet("hour")]
        public IActionResult GetHourHistory()
        {
            return Ok(_historyRepository.GetHourMeasurements());
        }
        [HttpGet("day")]
        public IActionResult GetDayHistory()
        {
            return Ok(_historyRepository.GetDayMeasurements());
        }
        [HttpGet("week")]
        public IActionResult GetWeekHistory()
        {
            return Ok(_historyRepository.GetWeekMeasurements());
        }
    }
}
