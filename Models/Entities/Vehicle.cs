using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Vehiculo")]
public class Vehicle
{
    [Key]
    [Column("PK_VehiculoID")]
    public int Id { get; set; }

    [Column("FK_ClienteID")]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("marca")]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("modelo")]
    public string Model { get; set; } = string.Empty;

    [Column("precio_venta")]
    public decimal Price { get; set; }

    [MaxLength(20)]
    [Column("moneda")]
    public string Currency { get; set; } = "SOLES";

    [ForeignKey("ClientId")]
    public Client? Client { get; set; }
    
    public ICollection<Credit> Credits { get; set; } = new List<Credit>();
}
