using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public class BilleteraService : IBilleteraService
{
    private readonly AppDbContext _db;

    public BilleteraService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Billetera?> GetByUsuario(int usuarioId)
    {
        return await _db.Billeteras
            .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);
    }

    public async Task<decimal> GetSaldo(int usuarioId)
    {
        var billetera = await GetByUsuario(usuarioId);
        return billetera?.Saldo ?? 0;
    }

    public async Task Depositar(int usuarioId, decimal monto)
    {
        var billetera = await GetByUsuario(usuarioId);
        if (billetera == null) return;

        billetera.Saldo += monto;

        var movimiento = new Movimiento
        {
            BilleteraId = billetera.Id,
            Tipo = "deposito",
            Monto = monto,
            Fecha = DateTime.UtcNow
        };
        _db.Movimientos.Add(movimiento);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Retirar(int usuarioId, decimal monto)
    {
        var billetera = await GetByUsuario(usuarioId);
        if (billetera == null || billetera.Saldo < monto)
            return false;

        billetera.Saldo -= monto;

        var movimiento = new Movimiento
        {
            BilleteraId = billetera.Id,
            Tipo = "retiro",
            Monto = -monto,
            Fecha = DateTime.UtcNow
        };
        _db.Movimientos.Add(movimiento);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Movimiento>> GetMovimientos(int usuarioId)
    {
        var billetera = await GetByUsuario(usuarioId);
        if (billetera == null) return new();

        return await _db.Movimientos
            .Where(m => m.BilleteraId == billetera.Id)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }
}
