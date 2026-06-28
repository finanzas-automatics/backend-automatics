using AutomaticsApi.Data;
using AutomaticsApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AutomaticsApi.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync();
}

public class DashboardService : IDashboardService
{
    private readonly AutomaticsDbContext _db;

    public DashboardService(AutomaticsDbContext db) => _db = db;

    public async Task<DashboardResponse> GetDashboardAsync()
    {
        var totalClients = await _db.Clients.CountAsync();
        
        // Mocking some values since the old statuses were removed
        var activeContracts = await _db.Credits.CountAsync(c => c.Status == "Aprobado");
        var inEvaluation = await _db.RiskEvaluations.CountAsync();
        var withOverdue = await _db.Credits.CountAsync(c => c.Status == "Mora");
        
        var totalFinanced = await _db.Credits.SumAsync(c => (decimal?)c.LoanAmount) ?? 0m;

        double approvalRate = totalClients == 0
            ? 0
            : Math.Round((double)activeContracts / totalClients * 100, 1);

        var recent = await _db.OperationLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(5)
            .Select(l => new RecentActivityResponse(
                l.Action,
                l.Module,
                0m,
                "Reciente"
            ))
            .ToListAsync();

        return new DashboardResponse(
            totalClients,
            activeContracts,
            inEvaluation,
            withOverdue,
            totalFinanced,
            approvalRate,
            recent
        );
    }
}
