using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public interface IAdminService
{
    Task<Partido> CrearPartido(Partido partido);
    Task<Partido?> EditarPartido(Partido partido);
    Task<Partido?> GetPartidoById(int id);
    Task<List<Partido>> GetAllPartidos();
    Task RegistrarResultado(int partidoId, int golesLocal, int golesVisitante);
    Task ToggleApuestas(int partidoId, bool habilitadas);
}
