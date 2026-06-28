using Microsoft.EntityFrameworkCore;
using AutomaticsApi.Models.Entities;

namespace AutomaticsApi.Data;

public class AutomaticsDbContext : DbContext
{
    public AutomaticsDbContext(DbContextOptions<AutomaticsDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();
    public DbSet<ClientProof> ClientProofs => Set<ClientProof>();
    public DbSet<RiskEvaluation> RiskEvaluations => Set<RiskEvaluation>();
    public DbSet<Credit> Credits => Set<Credit>();
    public DbSet<PaymentSchedule> PaymentSchedules => Set<PaymentSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        // Client
        modelBuilder.Entity<Client>(e =>
        {
            e.HasIndex(c => c.DocumentNumber).IsUnique();
            e.HasOne(c => c.User)
             .WithMany(u => u.Clients)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Vehicle
        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasOne(v => v.Client)
             .WithMany(c => c.Vehicles)
             .HasForeignKey(v => v.ClientId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // OperationLog
        modelBuilder.Entity<OperationLog>(e =>
        {
            e.HasOne(l => l.User)
             .WithMany(u => u.OperationLogs)
             .HasForeignKey(l => l.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ClientProof
        modelBuilder.Entity<ClientProof>(e =>
        {
            e.HasOne(p => p.Client)
             .WithMany(c => c.Proofs)
             .HasForeignKey(p => p.ClientId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // RiskEvaluation
        modelBuilder.Entity<RiskEvaluation>(e =>
        {
            e.HasOne(r => r.Client)
             .WithMany(c => c.RiskEvaluations)
             .HasForeignKey(r => r.ClientId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Credit
        modelBuilder.Entity<Credit>(e =>
        {
            e.HasOne(c => c.Client)
             .WithMany(cl => cl.Credits)
             .HasForeignKey(c => c.ClientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.Vehicle)
             .WithMany(v => v.Credits)
             .HasForeignKey(c => c.VehicleId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentSchedule
        modelBuilder.Entity<PaymentSchedule>(e =>
        {
            e.HasOne(ps => ps.Credit)
             .WithMany(c => c.PaymentSchedules)
             .HasForeignKey(ps => ps.CreditId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed: usuario administrador por defecto
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Name = "Carlos Méndez",
            Email = "carlos@automatics.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Asesor Senior de Crédito",
            Dni = "AM-2024-8832"
        });
    }
}
