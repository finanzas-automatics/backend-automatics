using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Usuario")]
public class User
{
    [Key]
    [Column("PK_UsuarioID")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nombre_completo")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("dni")]
    public string Dni { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("correo")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    // Although not in the strict list, keeping role is useful for the application UI
    [MaxLength(50)]
    [Column("role")]
    public string Role { get; set; } = "asesor";

    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();
}
