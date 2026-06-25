using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public interface IPartidoService
{
    Task<List<Partido>> GetAll();
    Task<List<Partido>> GetByFase(string fase);
    Task<Partido?> GetById(int id);
    Task<List<Partido>> GetProximos();
    Task<List<string>> GetFases();
}
