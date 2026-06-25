using sistemaapuestas.Models;

namespace sistemaapuestas.ViewModels.Apuestas;

public class HistorialViewModel
{
    public List<Apuesta> Apuestas { get; set; } = new();
    public string? FiltroEstado { get; set; }
    public decimal TotalGanado { get; set; }
    public decimal TotalApostado { get; set; }
}
