namespace AutomaticsApi.Models.DTOs;

public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data
);

// ─── AUTH ───────────────────────────────────────────────
public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string Token,
    string Name,
    string Email,
    string Role,
    string Dni,
    DateTime ExpiresAt
);

public record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string Dni,
    string Role = "Asesor Senior de Crédito"
);

// ─── CLIENT ─────────────────────────────────────────────
public record ClientCreateRequest(
    string DocumentType,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Address,
    decimal MonthlyIncome,
    VehicleCreateRequest? Vehicle
);

public record ClientUpdateRequest(
    string DocumentType,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Address,
    decimal MonthlyIncome,
    VehicleUpdateRequest? Vehicle
);

public record ClientResponse(
    int Id,
    string DocumentType,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string FullName,
    string? Email,
    string? Phone,
    string? Address,
    decimal MonthlyIncome,
    string Status,
    string CreatedAt,
    VehicleResponse? Vehicle
);

public record ClientListResponse(
    int Id,
    string FullName,
    string DocumentNumber,
    string? Email,
    string Status,
    string? VehicleName,
    decimal? VehiclePrice,
    string? VehicleCurrency
);

// ─── VEHICLE ────────────────────────────────────────────
public record VehicleCreateRequest(
    string Brand,
    string Model,
    decimal Price,
    string Currency
);

public record VehicleUpdateRequest(
    string Brand,
    string Model,
    decimal Price,
    string Currency
);

public record VehicleResponse(
    int Id,
    string Brand,
    string Model,
    int? Year,
    decimal Price,
    string Currency,
    string Status,
    string? FuelType,
    string? Transmission,
    string? Engine
);

// ─── DASHBOARD ──────────────────────────────────────────
public record DashboardResponse(
    int TotalClients,
    int ActiveContracts,
    int InEvaluation,
    int WithOverdue,
    decimal TotalFinanced,
    double ApprovalRate,
    List<RecentActivityResponse> RecentActivity
);

public record RecentActivityResponse(
    string ClientName,
    string Description,
    decimal Amount,
    string TimeAgo
);

// ─── SIMULATOR ──────────────────────────────────────────
public record SimulationRequest(
    decimal VehiclePrice,
    double InitialPaymentPct,
    double FinalPaymentPct,
    int TermMonths,
    string RateType,
    double RateValue,
    string? Capitalization,
    string GracePeriodType,
    int GraceMonths,
    double Cok
);

public record SimulationResponse(
    decimal LoanAmount,
    decimal InitialPayment,
    decimal BalloonPayment,
    decimal MonthlyPayment,
    double Tea,
    double TirMonthly,
    double Tcea,
    decimal Van,
    decimal TotalInterest,
    decimal TotalPayment,
    List<ScheduleRowResponse> Schedule
);

public record ScheduleRowResponse(
    int Month,
    decimal InitialBalance,
    decimal Interest,
    decimal Amortization,
    decimal Insurance,
    decimal TotalPayment,
    decimal FinalBalance,
    string GraceLabel
);

// ─── PAGINATION ─────────────────────────────────────────
public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int CurrentPage,
    int PageSize,
    int TotalPages
);
