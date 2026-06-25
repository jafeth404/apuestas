using System.ComponentModel.DataAnnotations;

namespace sistemaapuestas.ViewModels.Admin;

public class GestionPartidoViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "El equipo local es requerido")]
    [MaxLength(100)]
    public string EquipoLocal { get; set; }

    [Required(ErrorMessage = "El equipo visitante es requerido")]
    [MaxLength(100)]
    public string EquipoVisitante { get; set; }

    [Required(ErrorMessage = "La fecha y hora es requerida")]
    [DataType(DataType.DateTime)]
    public DateTime FechaHora { get; set; }

    [Required(ErrorMessage = "La fase es requerida")]
    [MaxLength(30)]
    public string Fase { get; set; }

    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }

    public string Estado { get; set; } = "programado";

    public bool ApuestasHabilitadas { get; set; } = true;

    public string? Accion { get; set; }
}
