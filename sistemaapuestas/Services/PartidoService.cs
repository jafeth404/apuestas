using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public class PartidoService : IPartidoService
{
    private readonly AppDbContext _db;

    private static readonly Dictionary<string, int> PhaseOrder = new()
    {
        ["Fase de Grupos - Grupo A"] = 1,
        ["Fase de Grupos - Grupo B"] = 2,
        ["Fase de Grupos - Grupo C"] = 3,
        ["Fase de Grupos - Grupo D"] = 4,
        ["Fase de Grupos - Grupo E"] = 5,
        ["Fase de Grupos - Grupo F"] = 6,
        ["Fase de Grupos - Grupo G"] = 7,
        ["Fase de Grupos - Grupo H"] = 8,
        ["Fase de Grupos - Grupo I"] = 9,
        ["Fase de Grupos - Grupo J"] = 10,
        ["Fase de Grupos - Grupo K"] = 11,
        ["Fase de Grupos - Grupo L"] = 12,
        ["Ronda de 32"] = 13,
        ["Octavos de Final"] = 14,
        ["Cuartos de Final"] = 15,
        ["Semifinal"] = 16,
        ["Tercer Puesto"] = 17,
        ["Final"] = 18
    };

    public PartidoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Partido>> GetAll()
    {
        return await _db.Partidos
            .Include(p => p.Cuotas)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Partido>> GetByFase(string fase)
    {
        return await _db.Partidos
            .Include(p => p.Cuotas)
            .Where(p => p.Fase == fase)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<Partido?> GetById(int id)
    {
        return await _db.Partidos
            .Include(p => p.Cuotas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Partido>> GetProximos()
    {
        return await _db.Partidos
            .Include(p => p.Cuotas)
            .Where(p => p.Estado == "programado" && p.FechaHora > DateTime.UtcNow)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<List<string>> GetFases()
    {
        var fases = await _db.Partidos
            .Select(p => p.Fase)
            .Distinct()
            .ToListAsync();

        return fases
            .OrderBy(f => PhaseOrder.TryGetValue(f, out var order) ? order : int.MaxValue)
            .ToList();
    }
}
