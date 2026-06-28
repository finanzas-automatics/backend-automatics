using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomaticsApi.Models.Entities;

[Table("Evaluacion_Riesgo")]
public class RiskEvaluation
{
    [Key]
    [Column("PK_EvaluacionID")]
    public int Id { get; set; }

    [Column("FK_ClienteID")]
    public int ClientId { get; set; }

    [Column("score_infocorp")]
    public int InfocorpScore { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("clasificacion_sbs")]
    public string SbsClassification { get; set; } = string.Empty;

    [Column("observaciones")]
    public string? Observations { get; set; }

    [ForeignKey("ClientId")]
    public Client? Client { get; set; }
}
