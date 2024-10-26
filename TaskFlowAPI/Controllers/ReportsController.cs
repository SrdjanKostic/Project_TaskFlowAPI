using Microsoft.AspNetCore.Mvc;
using TaskFlowAPI.Data;
using System.Threading.Tasks;

namespace TaskFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint za dobijanje broja zadataka po statusu
        [HttpGet("task-status-count")]
        public async Task<IActionResult> GetTaskStatusCount()
        {
            var taskStatusCount = await _context.GetTaskStatusCountAsync();
            return Ok(taskStatusCount);
        }

        // Endpoint za dobijanje sažetka produktivnosti
        [HttpGet("productivity-summary")]
        public async Task<IActionResult> GetProductivitySummary()
        {
            var productivitySummary = await _context.GetProductivitySummaryAsync();
            return Ok(productivitySummary);
        }
    }
}
