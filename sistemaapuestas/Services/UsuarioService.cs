using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _db;

    public UsuarioService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario> Registrar(string nombre, string correo, string contrasena, DateTime fechaNacimiento)
    {
        var usuario = new Usuario
        {
            Nombre = nombre,
            Correo = correo,
            Contrasena = BCryptHash(contrasena),
            FechaNacimiento = fechaNacimiento,
            Estado = "activo",
            FechaRegistro = DateTime.UtcNow
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        var billetera = new Billetera
        {
            UsuarioId = usuario.Id,
            Saldo = 0,
            Moneda = "USD"
        };

        _db.Billeteras.Add(billetera);
        await _db.SaveChangesAsync();

        return usuario;
    }

    public async Task<Usuario?> Login(string correo, string contrasena)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
        if (usuario == null || !BCryptVerify(contrasena, usuario.Contrasena))
            return null;

        return usuario;
    }

    public async Task<Usuario?> GetById(int id)
    {
        return await _db.Usuarios.FindAsync(id);
    }

    public async Task<bool> CorreoExiste(string correo)
    {
        return await _db.Usuarios.AnyAsync(u => u.Correo == correo);
    }

    private static string BCryptHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool BCryptVerify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
