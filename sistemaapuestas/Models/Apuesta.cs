using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("apuestas")]
public class Apuesta
{
    [Key]
    [Column("id_apuesta")]
    public int Id { get; set; }

    [Column("id_usuario")]
    [Required]
    public int UsuarioId { get; set; }

    [Column("id_partido")]
    [Required]
    public int PartidoId { get; set; }

    [Column("tipo_apuesta")]
    [Required, MaxLength(50)]
    public string TipoApuesta { get; set; }

    [Column("monto")]
    public decimal Monto { get; set; }

    [Column("cuota_aplicada")]
    public decimal CuotaAplicada { get; set; }

    [Column("estado")]
    [Required, MaxLength(20)]
    public string Estado { get; set; } = "pendiente";

    [Column("fecha_apuesta")]
    public DateTime FechaApuesta { get; set; } = DateTime.UtcNow;

    [Column("ganancia")]
    public decimal? Ganancia { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }

    [ForeignKey(nameof(PartidoId))]
    public Partido Partido { get; set; }
}
