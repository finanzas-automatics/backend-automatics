using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutomaticsApi.Models.DTOs;
using AutomaticsApi.Services;

namespace AutomaticsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService) => _clientService = clientService;

    /// <summary>Listar clientes con paginación, búsqueda y filtro por estado</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<ClientListResponse>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        var result = await _clientService.GetAllAsync(page, pageSize, search, status);
        return Ok(new ApiResponse<PagedResponse<ClientListResponse>>(true, "OK", result));
    }

    /// <summary>Obtener cliente por ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ClientResponse>>> GetById(int id)
    {
        var client = await _clientService.GetByIdAsync(id);
        if (client is null)
            return NotFound(new ApiResponse<ClientResponse>(false, "Cliente no encontrado", null));

        return Ok(new ApiResponse<ClientResponse>(true, "OK", client));
    }

    /// <summary>Crear nuevo cliente</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClientResponse>>> Create([FromBody] ClientCreateRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        var client = await _clientService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = client.Id },
            new ApiResponse<ClientResponse>(true, "Cliente creado", client));
    }

    /// <summary>Actualizar cliente</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ClientResponse>>> Update(
        int id, [FromBody] ClientUpdateRequest request)
    {
        var client = await _clientService.UpdateAsync(id, request);
        if (client is null)
            return NotFound(new ApiResponse<ClientResponse>(false, "Cliente no encontrado", null));

        return Ok(new ApiResponse<ClientResponse>(true, "Cliente actualizado", client));
    }

    /// <summary>Eliminar cliente</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var ok = await _clientService.DeleteAsync(id);
        if (!ok)
            return NotFound(new ApiResponse<string>(false, "Cliente no encontrado", null));

        return Ok(new ApiResponse<string>(true, "Cliente eliminado", null));
    }
}
