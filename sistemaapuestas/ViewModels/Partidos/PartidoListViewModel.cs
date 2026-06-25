using sistemaapuestas.Models;

namespace sistemaapuestas.ViewModels.Partidos;

public class PartidoListViewModel
{
    public string Fase { get; set; }
    public List<Partido> Partidos { get; set; } = new();
}
