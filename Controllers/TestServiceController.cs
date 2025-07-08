
using System.Text;
using DateInputNormalizer.DateNormalization;
using DateInputNormalizer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DateInputNormalizer.Controllers
{

    [ApiController]
    public class TestServiceController : ControllerBase
    {
        private DateLogic _dateLogic;

        public TestServiceController() 
        {
            _dateLogic = new DateLogic();
        }

        [HttpPost]
        [Route("api/[controller]/TestDateHandling")]
        public IActionResult TestDateHandling(TestDateModel incoming)
        {
            var result = new TestDateModel()
            {
                skipsOtherProperty = false,
                ReceivedDateTimeRaw = incoming.EventTime.ToString("O"),
                ReceivedDateOnlyRaw = incoming.EventDate.ToString("yyyy-MM-dd"),
                EventTime = incoming.EventTime,
                EventDate = incoming.EventDate,
                ConvertedDateOnly = DateOnly.FromDateTime(_dateLogic.ToUtcDateTimeSafe(incoming.EventDate)),
                ConvertedSafeDateOnly = _dateLogic.ToLocalDateSafe((_dateLogic.ToUtcDateTimeSafe(incoming.EventDate)))
            };
            return Ok(result);
        }

    }
}
