using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    private readonly IApuestaService _apuestaService;

    public AdminService(AppDbContext db, IApuestaService apuestaService)
    {
        _db = db;
        _apuestaService = apuestaService;
    }

    public async Task<Partido> CrearPartido(Partido partido)
    {
        _db.Partidos.Add(partido);
        await _db.SaveChangesAsync();
        return partido;
    }

    public async Task<Partido?> EditarPartido(Partido partido)
    {
        var existente = await _db.Partidos.FindAsync(partido.Id);
        if (existente == null) return null;

        existente.EquipoLocal = partido.EquipoLocal;
        existente.EquipoVisitante = partido.EquipoVisitante;
        existente.FechaHora = partido.FechaHora;
        existente.Fase = partido.Fase;
        existente.ApuestasHabilitadas = partido.ApuestasHabilitadas;
        await _db.SaveChangesAsync();
        return existente;
    }

    public async Task<Partido?> GetPartidoById(int id)
    {
        return await _db.Partidos.FindAsync(id);
    }

    public async Task<List<Partido>> GetAllPartidos()
    {
        return await _db.Partidos
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task RegistrarResultado(int partidoId, int golesLocal, int golesVisitante)
    {
        var partido = await _db.Partidos.FindAsync(partidoId);
        if (partido == null) return;

        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        partido.Estado = "finalizado";
        partido.ApuestasHabilitadas = false;
        await _db.SaveChangesAsync();

        await _apuestaService.LiquidarApuestas(partidoId, golesLocal, golesVisitante);
    }

    public async Task ToggleApuestas(int partidoId, bool habilitadas)
    {
        var partido = await _db.Partidos.FindAsync(partidoId);
        if (partido != null)
        {
            partido.ApuestasHabilitadas = habilitadas;
            await _db.SaveChangesAsync();
        }
    }
}
