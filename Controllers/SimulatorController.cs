using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutomaticsApi.Models.DTOs;
using AutomaticsApi.Services;

namespace AutomaticsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SimulatorController : ControllerBase
{
    private readonly ISimulatorService _simulatorService;

    public SimulatorController(ISimulatorService simulatorService)
        => _simulatorService = simulatorService;

    /// <summary>
    /// Calcular simulación de crédito vehicular.
    /// Método Francés Vencido Ordinario — meses de 30 días.
    /// Soporta Compra Inteligente (cuota balloon), gracia parcial y total.
    /// Devuelve: cuota mensual, VAN, TIR, TCEA y cronograma completo.
    /// </summary>
    [HttpPost("calculate")]
    public ActionResult<ApiResponse<SimulationResponse>> Calculate([FromBody] SimulationRequest request)
    {
        if (request.VehiclePrice <= 0)
            return BadRequest(new ApiResponse<SimulationResponse>(false, "Precio del vehículo inválido", null));

        if (request.InitialPaymentPct is < 0 or > 100)
            return BadRequest(new ApiResponse<SimulationResponse>(false, "Cuota inicial debe estar entre 0 y 100%", null));

        if (request.RateValue <= 0)
            return BadRequest(new ApiResponse<SimulationResponse>(false, "La tasa debe ser mayor a 0", null));

        if (request.TermMonths is < 1 or > 120)
            return BadRequest(new ApiResponse<SimulationResponse>(false, "El plazo debe estar entre 1 y 120 meses", null));

        var result = _simulatorService.Calculate(request);
        return Ok(new ApiResponse<SimulationResponse>(true, "Simulación calculada", result));
    }
}
