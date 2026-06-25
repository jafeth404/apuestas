using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("cuotas")]
public class Cuota
{
    [Key]
    [Column("id_cuota")]
    public int Id { get; set; }

    [Column("id_partido")]
    [Required]
    public int PartidoId { get; set; }

    [Column("tipo")]
    [Required, MaxLength(50)]
    public string Tipo { get; set; }

    [Column("valor")]
    public decimal Valor { get; set; }

    [Column("fecha_actualizacion")]
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(PartidoId))]
    public Partido Partido { get; set; }
}
