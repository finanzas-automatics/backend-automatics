using AutomaticsApi.Data;
using AutomaticsApi.Models.DTOs;
using AutomaticsApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutomaticsApi.Services;

public interface IClientService
{
    Task<PagedResponse<ClientListResponse>> GetAllAsync(int page, int pageSize, string? search, string? status);
    Task<ClientResponse?> GetByIdAsync(int id);
    Task<ClientResponse> CreateAsync(ClientCreateRequest request, int userId);
    Task<ClientResponse?> UpdateAsync(int id, ClientUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}

public class ClientService : IClientService
{
    private readonly AutomaticsDbContext _db;

    public ClientService(AutomaticsDbContext db) => _db = db;

    public async Task<PagedResponse<ClientListResponse>> GetAllAsync(
        int page, int pageSize, string? search, string? status)
    {
        var query = _db.Clients
            .Include(c => c.Vehicles)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(s) ||
                c.LastName.ToLower().Contains(s) ||
                c.DocumentNumber.Contains(s) ||
                (c.Email != null && c.Email.ToLower().Contains(s)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ClientListResponse(
                c.Id,
                $"{c.FirstName} {c.LastName}",
                c.DocumentNumber,
                c.Email,
                "activo", // Mock Status
                c.Vehicles.Any()
                    ? $"{c.Vehicles.First().Brand} {c.Vehicles.First().Model}"
                    : null,
                c.Vehicles.Any() ? c.Vehicles.First().Price : null,
                c.Vehicles.Any() ? c.Vehicles.First().Currency : null
            ))
            .ToListAsync();

        return new PagedResponse<ClientListResponse>(
            items, total, page, pageSize,
            (int)Math.Ceiling((double)total / pageSize));
    }

    public async Task<ClientResponse?> GetByIdAsync(int id)
    {
        var client = await _db.Clients
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == id);

        return client is null ? null : MapToResponse(client);
    }

    public async Task<ClientResponse> CreateAsync(ClientCreateRequest request, int userId)
    {
        var client = new Client
        {
            UserId = userId,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            MonthlyIncome = request.MonthlyIncome
        };

        if (request.Vehicle is not null)
        {
            client.Vehicles.Add(new Vehicle
            {
                Brand = request.Vehicle.Brand,
                Model = request.Vehicle.Model,
                Price = request.Vehicle.Price,
                Currency = request.Vehicle.Currency
            });
        }

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return MapToResponse(client);
    }

    public async Task<ClientResponse?> UpdateAsync(int id, ClientUpdateRequest request)
    {
        var client = await _db.Clients
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client is null) return null;

        client.DocumentType = request.DocumentType;
        client.DocumentNumber = request.DocumentNumber;
        client.FirstName = request.FirstName;
        client.LastName = request.LastName;
        client.Email = request.Email;
        client.Phone = request.Phone;
        client.Address = request.Address;
        client.MonthlyIncome = request.MonthlyIncome;

        if (request.Vehicle is not null)
        {
            var vehicle = client.Vehicles.FirstOrDefault();
            if (vehicle is not null)
            {
                vehicle.Brand = request.Vehicle.Brand;
                vehicle.Model = request.Vehicle.Model;
                vehicle.Price = request.Vehicle.Price;
                vehicle.Currency = request.Vehicle.Currency;
            }
            else
            {
                client.Vehicles.Add(new Vehicle
                {
                    Brand = request.Vehicle.Brand,
                    Model = request.Vehicle.Model,
                    Price = request.Vehicle.Price,
                    Currency = request.Vehicle.Currency
                });
            }
        }

        await _db.SaveChangesAsync();
        return MapToResponse(client);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client is null) return false;
        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return true;
    }

    private static ClientResponse MapToResponse(Client c)
    {
        var v = c.Vehicles.FirstOrDefault();
        return new ClientResponse(
            c.Id,
            c.DocumentType,
            c.DocumentNumber,
            c.FirstName,
            c.LastName,
            $"{c.FirstName} {c.LastName}",
            c.Email,
            c.Phone,
            c.Address,
            c.MonthlyIncome,
            "activo", // Status
            DateTime.UtcNow.ToString("O"), // CreatedAt
            v is null ? null : new VehicleResponse(
                v.Id, v.Brand, v.Model, 2023, v.Price, v.Currency, "disponible", "Gasolina", "Automatica", "1.9L")
        );
    }
}
