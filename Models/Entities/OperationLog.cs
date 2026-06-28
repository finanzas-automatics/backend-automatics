using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Log_Operacion")]
public class OperationLog
{
    [Key]
    [Column("PK_LogID")]
    public int Id { get; set; }

    [Column("FK_UsuarioID")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("accion")]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("modulo")]
    public string Module { get; set; } = string.Empty;

    [Column("fecha_hora")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Column("descripcion_detallada")]
    public string? Description { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
