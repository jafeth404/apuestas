using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Data;
using sistemaapuestas.Filters;
using sistemaapuestas.Services;
using sistemaapuestas.ViewModels.Apuestas;

namespace sistemaapuestas.Controllers;

public class ApuestasController : Controller
{
    private readonly IApuestaService _apuestaService;
    private readonly IPartidoService _partidoService;
    private readonly IBilleteraService _billeteraService;
    private readonly AppDbContext _db;

    public ApuestasController(
        IApuestaService apuestaService,
        IPartidoService partidoService,
        IBilleteraService billeteraService,
        AppDbContext db)
    {
        _apuestaService = apuestaService;
        _partidoService = partidoService;
        _billeteraService = billeteraService;
        _db = db;
    }

    [RequireAuth]
    public async Task<IActionResult> Create(int partidoId)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        var partido = await _partidoService.GetById(partidoId);
        if (partido == null) return NotFound();

        var cuota = partido.Cuotas.FirstOrDefault();

        var viewModel = new CrearApuestaViewModel
        {
            PartidoId = partido.Id,
            EquipoLocal = partido.EquipoLocal,
            EquipoVisitante = partido.EquipoVisitante,
            Cuota = cuota?.Valor ?? 1.0m,
            CuotaDisplay = $"{partido.EquipoLocal} ({(cuota?.Valor ?? 1):F2})"
        };

        ViewBag.TiposApuesta = new SelectList(
            new[] { "local", "visitante", "empate" },
            viewModel.TipoApuesta);

        return View(viewModel);
    }

    [HttpPost]
    [RequireAuth]
    public async Task<IActionResult> Create(CrearApuestaViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        if (!ModelState.IsValid)
            return View(model);

        var partido = await _partidoService.GetById(model.PartidoId);
        if (partido == null || partido.Estado != "programado" || !partido.ApuestasHabilitadas)
        {
            ModelState.AddModelError("", "El partido no acepta apuestas en este momento");
            return View(model);
        }

        var saldo = await _billeteraService.GetSaldo(userId);
        if (saldo < model.Monto)
        {
            ModelState.AddModelError("Monto", "Saldo insuficiente");
            return View(model);
        }

        var cuota = partido.Cuotas.FirstOrDefault()?.Valor ?? 1.0m;
        model.Cuota = cuota;
        model.GananciaPotencial = model.Monto * cuota;

        await _apuestaService.CrearApuesta(userId, model.PartidoId, model.TipoApuesta, model.Monto, cuota);

        TempData["Success"] = "Apuesta registrada exitosamente";
        return RedirectToAction("Historial");
    }

    [RequireAuth]
    public async Task<IActionResult> Historial(string? estado)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var apuestas = await _apuestaService.GetByUsuarioFiltro(userId, estado);

        var viewModel = new HistorialViewModel
        {
            Apuestas = apuestas,
            FiltroEstado = estado,
            TotalApostado = apuestas.Sum(a => a.Monto),
            TotalGanado = apuestas.Where(a => a.Estado == "ganada").Sum(a => a.Ganancia ?? 0)
        };

        return View(viewModel);
    }
}
