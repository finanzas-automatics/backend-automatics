using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Credito")]
public class Credit
{
    [Key]
    [Column("PK_CreditoID")]
    public int Id { get; set; }

    [Column("FK_ClienteID")]
    public int ClientId { get; set; }

    [Column("FK_VehiculoID")]
    public int VehicleId { get; set; }

    [Column("monto_prestamo")]
    public decimal LoanAmount { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("tipo_tasa")]
    public string RateType { get; set; } = string.Empty;

    [Column("valor_tasa")]
    public decimal RateValue { get; set; }

    [MaxLength(20)]
    [Column("capitalizacion")]
    public string? Capitalization { get; set; }

    [Column("plazo_meses")]
    public int TermMonths { get; set; }

    [Column("cuota_final_inteligente_porcentaje")]
    public decimal SmartFinalInstallmentPercentage { get; set; }

    [MaxLength(20)]
    [Column("periodo_gracia")]
    public string? GracePeriod { get; set; }

    [Column("indicadores")]
    public string? IndicatorsJson { get; set; } // Can store JSON object with VAN, TIR, TCEA

    [Required]
    [MaxLength(50)]
    [Column("estado")]
    public string Status { get; set; } = "Pendiente";

    [ForeignKey("ClientId")]
    public Client? Client { get; set; }

    [ForeignKey("VehicleId")]
    public Vehicle? Vehicle { get; set; }

    public ICollection<PaymentSchedule> PaymentSchedules { get; set; } = new List<PaymentSchedule>();
}
