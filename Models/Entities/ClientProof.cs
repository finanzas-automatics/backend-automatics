using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Sustento_Cliente")]
public class ClientProof
{
    [Key]
    [Column("PK_SustentoID")]
    public int Id { get; set; }

    [Column("FK_ClienteID")]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("tipo_documento")]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("url_archivo")]
    public string FileUrl { get; set; } = string.Empty;

    [Column("verificado")]
    public bool IsVerified { get; set; } = false;

    [ForeignKey("ClientId")]
    public Client? Client { get; set; }
}
