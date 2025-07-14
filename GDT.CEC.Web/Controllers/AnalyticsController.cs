using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GDT.CEC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<UsersController> logger)
        {
            _analyticsService = analyticsService;
        }

        [HttpPost("insertlog")]
        public async Task<IActionResult> InsertLog(LabAccessLogDTO logDTO)
        {
            var response = await _analyticsService.CreateAsync(logDTO);

            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<List<LabAccessLogDTO>>()
            {
                Message = "",
                StatusCode=1
            });

        }

        [Authorize]
        [HttpPost("getlabaccesslogs")]
        public async Task<IActionResult> GetAccessLogs(PagingModel model)
        {
            var response = await _analyticsService.GetLabAccessLogsAsync(model.pageno, model.pagesize);

            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<LabAcccessLogPagingModel>()
            {
                Message = "",
                Data = response
            });           
        }

        [Authorize]
        [HttpGet("getlabanalytics")]
        public async Task<IActionResult> GetLabAnalytics(string filter="all")
        {
            var response = await _analyticsService.GetLabAccessLogCounts(filter);

            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<LabAccessLogCountDTO>()
            {
                Message = "",
                Data = response
            });
        }
    }
}
