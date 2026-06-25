using sistemaapuestas.Models;

namespace sistemaapuestas.ViewModels.Partidos;

public class PartidoDetalleViewModel
{
    public Partido Partido { get; set; }
    public List<Cuota> Cuotas { get; set; } = new();
    public bool PuedeApostar { get; set; }
}
