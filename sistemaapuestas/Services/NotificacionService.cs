using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public class NotificacionService : INotificacionService
{
    private readonly AppDbContext _db;

    public NotificacionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Notificacion>> GetByUsuario(int usuarioId)
    {
        return await _db.Notificaciones
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();
    }

    public async Task<int> GetNoLeidas(int usuarioId)
    {
        return await _db.Notificaciones
            .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida);
    }

    public async Task Crear(int usuarioId, string mensaje, string tipo)
    {
        var notificacion = new Notificacion
        {
            UsuarioId = usuarioId,
            Mensaje = mensaje,
            Tipo = tipo,
            Leida = false,
            FechaCreacion = DateTime.UtcNow
        };
        _db.Notificaciones.Add(notificacion);
        await _db.SaveChangesAsync();
    }

    public async Task MarcarLeida(int notificacionId)
    {
        var notif = await _db.Notificaciones.FindAsync(notificacionId);
        if (notif != null)
        {
            notif.Leida = true;
            await _db.SaveChangesAsync();
        }
    }
}
