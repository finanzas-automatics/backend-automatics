using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Cronograma_Pago")]
public class PaymentSchedule
{
    [Key]
    [Column("PK_CronogramaID")]
    public int Id { get; set; }

    [Column("FK_CreditoID")]
    public int CreditId { get; set; }

    [Column("numero_mes")]
    public int MonthNumber { get; set; }

    [Column("saldo_inicial")]
    public decimal InitialBalance { get; set; }

    [Column("interes")]
    public decimal Interest { get; set; }

    [Column("amortizacion")]
    public decimal Amortization { get; set; }

    [Column("seguros_y_gastos")]
    public decimal InsuranceAndExpenses { get; set; }

    [Column("cuota_total_mensual")]
    public decimal TotalMonthlyInstallment { get; set; }

    [Column("saldo_final")]
    public decimal FinalBalance { get; set; }

    [ForeignKey("CreditId")]
    public Credit? Credit { get; set; }
}
