using insuranceclaimproject.Dtos;
using insuranceclaimproject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SummaryController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public SummaryController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var result = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(result);
        }
    }
}
