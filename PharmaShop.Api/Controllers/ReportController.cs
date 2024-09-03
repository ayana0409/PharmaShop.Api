using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Response.Report;

namespace PharmaShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("weekly-revenue")]
        public async Task<ActionResult<List<RevenueRecordResponse>>> GetWeeklyRevenue()
        {
            try
            {
                var weeklyRevenue = await _reportService.CalculateWeeklyRevenue();

                return Ok(weeklyRevenue);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
