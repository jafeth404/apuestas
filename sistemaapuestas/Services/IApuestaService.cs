using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public interface IApuestaService
{
    Task<Apuesta> CrearApuesta(int usuarioId, int partidoId, string tipoApuesta, decimal monto, decimal cuota);
    Task<List<Apuesta>> GetByUsuario(int usuarioId);
    Task<List<Apuesta>> GetByUsuarioFiltro(int usuarioId, string? estado);
    Task<List<Apuesta>> GetByPartido(int partidoId);
    Task LiquidarApuestas(int partidoId, int golesLocal, int golesVisitante);
}
