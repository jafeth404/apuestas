using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public class PartidoService : IPartidoService
{
    private readonly AppDbContext _db;

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
        return await _db.Partidos
            .Select(p => p.Fase)
            .Distinct()
            .OrderBy(f => f)
            .ToListAsync();
    }
}
