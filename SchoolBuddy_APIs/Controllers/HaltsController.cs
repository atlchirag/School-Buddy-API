using Microsoft.AspNetCore.Mvc;
using SchoolBuddy_APIs.Services;

namespace SchoolBuddy_APIs.Controllers
{
    [ApiController]
    [Route("api/halts")]
    public class HaltsController : ControllerBase
    {
        private readonly HaltsTelemetryService _svc;
        public HaltsController(HaltsTelemetryService svc) => _svc = svc;

        [HttpGet("all-schools")]
        public async Task<IActionResult> AllSchools() =>
            Ok(await _svc.GetAllSchoolsAsync());

        [HttpGet("school-routes/{schoolId:int}")]
        public async Task<IActionResult> RoutesForSchool(int schoolId) =>
            Ok(await _svc.GetRoutesForSchoolAsync(schoolId));

        [HttpGet("route-telemetry")]
        public async Task<IActionResult> RouteTelemetry([FromQuery] int schoolId, [FromQuery] int routeId, [FromQuery] DateTime date, [FromQuery] string pass, [FromQuery] string user) =>
            Ok(await _svc.GetRouteTelemetryAsync(schoolId, routeId, date,pass,user));

        [HttpGet("route-multiday-telemetry")]
        public async Task<IActionResult> MultiDay([FromQuery] int schoolId, [FromQuery] int routeId,
                                                  [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string pass, [FromQuery] string user) =>
            Ok(await _svc.GetMultiDayTelemetryAsync(schoolId, routeId, startDate, endDate,pass,user));
    }
}