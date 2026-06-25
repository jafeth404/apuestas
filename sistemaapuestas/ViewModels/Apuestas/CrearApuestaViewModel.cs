using System.ComponentModel.DataAnnotations;
using sistemaapuestas.Models;

namespace sistemaapuestas.ViewModels.Apuestas;

public class CrearApuestaViewModel
{
    public int PartidoId { get; set; }
    public string? EquipoLocal { get; set; }
    public string? EquipoVisitante { get; set; }

    [Required(ErrorMessage = "Seleccione un tipo de apuesta")]
    public string TipoApuesta { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(1, 999999, ErrorMessage = "El monto debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    public decimal Monto { get; set; }

    public decimal Cuota { get; set; }
    public decimal GananciaPotencial { get; set; }
    public string? CuotaDisplay { get; set; }
}
