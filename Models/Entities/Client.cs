using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Cliente")]
public class Client
{
    [Key]
    [Column("PK_ClienteID")]
    public int Id { get; set; }

    [Column("FK_UsuarioID")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("tipo_documento")]
    public string DocumentType { get; set; } = "DNI";

    [Required]
    [MaxLength(20)]
    [Column("numero_documento")]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombres")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("apellidos")]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(150)]
    [Column("correo")]
    public string? Email { get; set; }

    [MaxLength(20)]
    [Column("telefono")]
    public string? Phone { get; set; }

    [MaxLength(255)]
    [Column("direccion")]
    public string? Address { get; set; }

    [Column("ingresos_netos_mensuales")]
    public decimal MonthlyIncome { get; set; }

    [Column("score_interno")]
    public int Score { get; set; } = 0;

    [ForeignKey("UserId")]
    public User? User { get; set; }

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<ClientProof> Proofs { get; set; } = new List<ClientProof>();
    public ICollection<RiskEvaluation> RiskEvaluations { get; set; } = new List<RiskEvaluation>();
    public ICollection<Credit> Credits { get; set; } = new List<Credit>();
}
