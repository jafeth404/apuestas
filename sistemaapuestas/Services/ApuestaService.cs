using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public class ApuestaService : IApuestaService
{
    private readonly AppDbContext _db;
    private readonly INotificacionService _notificacionService;

    public ApuestaService(AppDbContext db, INotificacionService notificacionService)
    {
        _db = db;
        _notificacionService = notificacionService;
    }

    public async Task<Apuesta> CrearApuesta(int usuarioId, int partidoId, string tipoApuesta, decimal monto, decimal cuota)
    {
        var apuesta = new Apuesta
        {
            UsuarioId = usuarioId,
            PartidoId = partidoId,
            TipoApuesta = tipoApuesta,
            Monto = monto,
            CuotaAplicada = cuota,
            Estado = "pendiente",
            FechaApuesta = DateTime.UtcNow
        };

        _db.Apuestas.Add(apuesta);

        var billetera = await _db.Billeteras.FirstAsync(b => b.UsuarioId == usuarioId);
        billetera.Saldo -= monto;

        var movimiento = new Movimiento
        {
            BilleteraId = billetera.Id,
            Tipo = "apuesta",
            Monto = -monto,
            Fecha = DateTime.UtcNow
        };
        _db.Movimientos.Add(movimiento);

        await _db.SaveChangesAsync();
        return apuesta;
    }

    public async Task<List<Apuesta>> GetByUsuario(int usuarioId)
    {
        return await _db.Apuestas
            .Include(a => a.Partido)
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.FechaApuesta)
            .ToListAsync();
    }

    public async Task<List<Apuesta>> GetByUsuarioFiltro(int usuarioId, string? estado)
    {
        var query = _db.Apuestas
            .Include(a => a.Partido)
            .Where(a => a.UsuarioId == usuarioId);

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(a => a.Estado == estado);

        return await query
            .OrderByDescending(a => a.FechaApuesta)
            .ToListAsync();
    }

    public async Task<List<Apuesta>> GetByPartido(int partidoId)
    {
        return await _db.Apuestas
            .Include(a => a.Usuario)
            .Where(a => a.PartidoId == partidoId && a.Estado == "pendiente")
            .ToListAsync();
    }

    public async Task LiquidarApuestas(int partidoId, int golesLocal, int golesVisitante)
    {
        var apuestas = await _db.Apuestas
            .Include(a => a.Usuario)
            .ThenInclude(u => u.Billetera)
            .Where(a => a.PartidoId == partidoId && a.Estado == "pendiente")
            .ToListAsync();

        foreach (var apuesta in apuestas)
        {
            bool gano = EvaluarApuesta(apuesta.TipoApuesta, golesLocal, golesVisitante);
            apuesta.Estado = gano ? "ganada" : "perdida";
            apuesta.Ganancia = gano ? apuesta.Monto * apuesta.CuotaAplicada : 0;

            if (gano && apuesta.Usuario?.Billetera != null)
            {
                apuesta.Usuario.Billetera.Saldo += apuesta.Ganancia.Value;

                var movimiento = new Movimiento
                {
                    BilleteraId = apuesta.Usuario.Billetera.Id,
                    Tipo = "ganancia",
                    Monto = apuesta.Ganancia.Value,
                    Fecha = DateTime.UtcNow
                };
                _db.Movimientos.Add(movimiento);

                await _notificacionService.Crear(
                    apuesta.UsuarioId,
                    $"Apuesta ganada: {apuesta.TipoApuesta} - Ganaste ${apuesta.Ganancia.Value:F2}",
                    "apuesta_ganada");
            }
            else
            {
                await _notificacionService.Crear(
                    apuesta.UsuarioId,
                    $"Apuesta perdida: {apuesta.TipoApuesta} - Perdiste ${apuesta.Monto:F2}",
                    "apuesta_perdida");
            }
        }

        await _db.SaveChangesAsync();
    }

    private static bool EvaluarApuesta(string tipo, int golesLocal, int golesVisitante)
    {
        return tipo switch
        {
            "local" => golesLocal > golesVisitante,
            "visitante" => golesVisitante > golesLocal,
            "empate" => golesLocal == golesVisitante,
            _ => false
        };
    }
}
