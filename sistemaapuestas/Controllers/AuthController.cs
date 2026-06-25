using Microsoft.AspNetCore.Mvc;
using sistemaapuestas.Services;
using sistemaapuestas.ViewModels.Auth;

namespace sistemaapuestas.Controllers;

public class AuthController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var edad = DateTime.Today.Year - model.FechaNacimiento.Year;
        if (model.FechaNacimiento > DateTime.Today.AddYears(-edad)) edad--;
        if (edad < 18)
        {
            ModelState.AddModelError("FechaNacimiento", "Debe ser mayor de 18 años");
            return View(model);
        }

        if (await _usuarioService.CorreoExiste(model.Correo))
        {
            ModelState.AddModelError("Correo", "El correo ya está registrado");
            return View(model);
        }

        var usuario = await _usuarioService.Registrar(
            model.Nombre, model.Correo, model.Contrasena, model.FechaNacimiento);

        HttpContext.Session.SetInt32("UserId", usuario.Id);
        HttpContext.Session.SetString("UserName", usuario.Nombre);
        HttpContext.Session.SetString("UserRole", "usuario");

        TempData["Success"] = $"Bienvenido {usuario.Nombre}, cuenta creada exitosamente.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = await _usuarioService.Login(model.Correo, model.Contrasena);
        if (usuario == null)
        {
            ModelState.AddModelError("", "Correo o contraseña incorrectos");
            return View(model);
        }

        if (usuario.Estado == "suspendido")
        {
            ModelState.AddModelError("", "La cuenta está suspendida");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", usuario.Id);
        HttpContext.Session.SetString("UserName", usuario.Nombre);
        HttpContext.Session.SetString("UserRole", "usuario");

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
