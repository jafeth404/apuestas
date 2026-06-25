using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public interface INotificacionService
{
    Task<List<Notificacion>> GetByUsuario(int usuarioId);
    Task<int> GetNoLeidas(int usuarioId);
    Task Crear(int usuarioId, string mensaje, string tipo);
    Task MarcarLeida(int notificacionId);
}
