using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutomaticsApi.Models.DTOs;
using AutomaticsApi.Services;

namespace AutomaticsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
        => _dashboardService = dashboardService;

    /// <summary>KPIs y actividad reciente para el dashboard</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardResponse>>> Get()
    {
        var data = await _dashboardService.GetDashboardAsync();
        return Ok(new ApiResponse<DashboardResponse>(true, "OK", data));
    }
}
