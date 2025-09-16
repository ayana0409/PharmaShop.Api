using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Response.Report;
using System.Security.Claims;

namespace PharmaShop.Api.Controllers
{
    [Authorize]
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

        [HttpGet("product/{startDate}")]
        public async Task<ActionResult<ReportResponse<ProductReport>>> GetProductStatistic([FromRoute] DateTime startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized();
                }

                var response = await _reportService.GetProductStatisticsByDateAsync(username, startDate, endDate);

                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("customer/{startDate}")]
        public async Task<ActionResult<ReportResponse<CustomerReport>>> GetCustomerStatistic([FromRoute] DateTime startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized();
                }

                var response = await _reportService.GetCustomerStatisticsByDateAsync(username, startDate, endDate);

                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
