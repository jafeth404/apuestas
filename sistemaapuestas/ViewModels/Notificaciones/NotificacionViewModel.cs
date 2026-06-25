using sistemaapuestas.Models;

namespace sistemaapuestas.ViewModels.Notificaciones;

public class NotificacionViewModel
{
    public List<Notificacion> Notificaciones { get; set; } = new();
    public int NoLeidas { get; set; }
}
