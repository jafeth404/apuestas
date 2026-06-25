using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Filters;
using sistemaapuestas.Models;
using sistemaapuestas.Services;
using sistemaapuestas.ViewModels.Admin;

namespace sistemaapuestas.Controllers;

[RequireAdmin]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IPartidoService _partidoService;
    private readonly AppDbContext _db;

    public AdminController(IAdminService adminService, IPartidoService partidoService, AppDbContext db)
    {
        _adminService = adminService;
        _partidoService = partidoService;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var partidos = await _adminService.GetAllPartidos();
        return View(partidos);
    }

    public IActionResult CreatePartido()
    {
        ViewBag.Fases = new SelectList(new[]
        {
            "Fase de Grupos - Grupo A", "Fase de Grupos - Grupo B", "Fase de Grupos - Grupo C",
            "Fase de Grupos - Grupo D", "Fase de Grupos - Grupo E", "Fase de Grupos - Grupo F",
            "Fase de Grupos - Grupo G", "Fase de Grupos - Grupo H", "Fase de Grupos - Grupo I",
            "Fase de Grupos - Grupo J", "Fase de Grupos - Grupo K", "Fase de Grupos - Grupo L",
            "Ronda de 32", "Octavos de Final", "Cuartos de Final",
            "Semifinal", "Tercer Puesto", "Final"
        });

        return View(new GestionPartidoViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreatePartido(GestionPartidoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var partido = new Partido
        {
            EquipoLocal = model.EquipoLocal,
            EquipoVisitante = model.EquipoVisitante,
            FechaHora = model.FechaHora,
            Fase = model.Fase,
            Estado = "programado",
            ApuestasHabilitadas = true
        };

        await _adminService.CrearPartido(partido);
        TempData["Success"] = "Partido creado exitosamente";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditPartido(int id)
    {
        var partido = await _adminService.GetPartidoById(id);
        if (partido == null) return NotFound();

        var model = new GestionPartidoViewModel
        {
            Id = partido.Id,
            EquipoLocal = partido.EquipoLocal,
            EquipoVisitante = partido.EquipoVisitante,
            FechaHora = partido.FechaHora,
            Fase = partido.Fase,
            GolesLocal = partido.GolesLocal,
            GolesVisitante = partido.GolesVisitante,
            Estado = partido.Estado,
            ApuestasHabilitadas = partido.ApuestasHabilitadas,
            Accion = "editar"
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditPartido(GestionPartidoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var partido = new Partido
        {
            Id = model.Id!.Value,
            EquipoLocal = model.EquipoLocal,
            EquipoVisitante = model.EquipoVisitante,
            FechaHora = model.FechaHora,
            Fase = model.Fase,
            ApuestasHabilitadas = model.ApuestasHabilitadas
        };

        await _adminService.EditarPartido(partido);
        TempData["Success"] = "Partido actualizado exitosamente";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> RegistrarResultado(int id)
    {
        var partido = await _partidoService.GetById(id);
        if (partido == null) return NotFound();

        var model = new GestionPartidoViewModel
        {
            Id = partido.Id,
            EquipoLocal = partido.EquipoLocal,
            EquipoVisitante = partido.EquipoVisitante,
            Fase = partido.Fase,
            Accion = "resultado"
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarResultado(GestionPartidoViewModel model)
    {
        if (model.GolesLocal == null || model.GolesVisitante == null)
        {
            ModelState.AddModelError("", "Debe ingresar el marcador");
            return View(model);
        }

        await _adminService.RegistrarResultado(model.Id!.Value, model.GolesLocal.Value, model.GolesVisitante.Value);
        TempData["Success"] = "Resultado registrado. Apuestas liquidadas automáticamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleApuestas(int id, bool habilitadas)
    {
        await _adminService.ToggleApuestas(id, habilitadas);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SeedFromApi()
    {
        try
        {
            var service = HttpContext.RequestServices.GetRequiredService<IWorldCupApiService>();
            var count = await service.SeedAllMatches();
            TempData["Success"] = $"Se importaron {count} partidos desde la API del Mundial 2026";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al importar partidos: {ex.Message}";
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await _db.Usuarios.OrderByDescending(u => u.FechaRegistro).ToListAsync();
        return View(usuarios);
    }

    public IActionResult CreateAdmin()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdmin(string nombre, string usuario, string contrasena, string rol)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
        {
            ModelState.AddModelError("", "Todos los campos son requeridos");
            return View();
        }

        if (await _db.Administradores.AnyAsync(a => a.Usuario == usuario))
        {
            ModelState.AddModelError("", "El nombre de usuario ya existe");
            return View();
        }

        var admin = new Administrador
        {
            Nombre = nombre,
            Usuario = usuario,
            Contrasena = BCrypt.Net.BCrypt.HashPassword(contrasena),
            Rol = rol ?? "admin"
        };

        _db.Administradores.Add(admin);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Administrador '{nombre}' creado exitosamente";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SuspenderUsuario(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            usuario.Estado = usuario.Estado == "activo" ? "suspendido" : "activo";
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Usuarios");
    }

    [HttpPost]
    public async Task<IActionResult> CambiarRol(int id, string rol)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            HttpContext.Session.SetString("UserRole", rol);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Usuarios");
    }
}
