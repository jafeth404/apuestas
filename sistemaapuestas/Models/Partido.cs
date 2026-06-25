using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("partidos")]
public class Partido
{
    [Key]
    [Column("id_partido")]
    public int Id { get; set; }

    [Column("equipo_local")]
    [Required, MaxLength(100)]
    public string EquipoLocal { get; set; }

    [Column("equipo_visitante")]
    [Required, MaxLength(100)]
    public string EquipoVisitante { get; set; }

    [Column("fecha_hora")]
    [Required]
    public DateTime FechaHora { get; set; }

    [Column("fase")]
    [Required, MaxLength(30)]
    public string Fase { get; set; }

    [Column("goles_local")]
    public int? GolesLocal { get; set; }

    [Column("goles_visitante")]
    public int? GolesVisitante { get; set; }

    [Column("estado")]
    [Required, MaxLength(20)]
    public string Estado { get; set; } = "programado";

    [Column("apuestas_habilitadas")]
    public bool ApuestasHabilitadas { get; set; } = true;

    public ICollection<Cuota> Cuotas { get; set; } = new List<Cuota>();
    public ICollection<Apuesta> Apuestas { get; set; } = new List<Apuesta>();
}
