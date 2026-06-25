using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public interface IBilleteraService
{
    Task<Billetera?> GetByUsuario(int usuarioId);
    Task<decimal> GetSaldo(int usuarioId);
    Task Depositar(int usuarioId, decimal monto);
    Task<bool> Retirar(int usuarioId, decimal monto);
    Task<List<Movimiento>> GetMovimientos(int usuarioId);
}
