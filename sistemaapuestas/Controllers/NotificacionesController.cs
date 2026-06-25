using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using Microsoft.AspNetCore.Mvc;
using sistemaapuestas.Filters;
using sistemaapuestas.Services;
using sistemaapuestas.ViewModels.Notificaciones;

namespace sistemaapuestas.Controllers;

[RequireAuth]
public class NotificacionesController : Controller
{
    private readonly INotificacionService _notificacionService;

    public NotificacionesController(INotificacionService notificacionService)
    {
        _notificacionService = notificacionService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        var notificaciones = await _notificacionService.GetByUsuario(userId);
        var noLeidas = await _notificacionService.GetNoLeidas(userId);

        var viewModel = new NotificacionViewModel
        {
            Notificaciones = notificaciones,
            NoLeidas = noLeidas
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        await _notificacionService.MarcarLeida(id);
        return RedirectToAction("Index");
    }
}
