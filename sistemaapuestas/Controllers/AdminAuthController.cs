using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Models;

namespace sistemaapuestas.Controllers;

public class AdminAuthController : Controller
{
    private readonly AppDbContext _db;

    public AdminAuthController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> LoginAdmin()
    {
        if (HttpContext.Session.GetString("UserRole") == "admin")
            return RedirectToAction("Index", "Admin");

        if (!await _db.Administradores.AnyAsync())
        {
            var admin = new Administrador
            {
                Nombre = "Administrador Principal",
                Usuario = "admin",
                Contrasena = BCrypt.Net.BCrypt.HashPassword("Admin2026!"),
                Rol = "superadmin"
            };
            _db.Administradores.Add(admin);

            var usuario = new Usuario
            {
                Nombre = "Usuario Demo",
                Correo = "demo@test.com",
                Contrasena = BCrypt.Net.BCrypt.HashPassword("Demo2026!"),
                FechaNacimiento = new DateTime(1990, 1, 1),
                Estado = "activo",
                FechaRegistro = DateTime.UtcNow
            };
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();

            _db.Billeteras.Add(new Billetera { UsuarioId = usuario.Id, Saldo = 10000, Moneda = "USD" });
            await _db.SaveChangesAsync();
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginAdmin(string usuario, string contrasena)
    {
        var admin = await _db.Administradores.FirstOrDefaultAsync(a => a.Usuario == usuario);
        if (admin == null || !BCrypt.Net.BCrypt.Verify(contrasena, admin.Contrasena))
        {
            ViewBag.Error = "Credenciales de administrador incorrectas";
            return View();
        }

        HttpContext.Session.SetInt32("UserId", admin.Id);
        HttpContext.Session.SetString("UserName", admin.Nombre);
        HttpContext.Session.SetString("UserRole", "admin");

        return RedirectToAction("Index", "Admin");
    }
}
