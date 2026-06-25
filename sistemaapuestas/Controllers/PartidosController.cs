using Microsoft.AspNetCore.Mvc;
using sistemaapuestas.Filters;
using sistemaapuestas.Services;

namespace sistemaapuestas.Controllers;

public class PartidosController : Controller
{
    private readonly IPartidoService _partidoService;

    public PartidosController(IPartidoService partidoService)
    {
        _partidoService = partidoService;
    }

    public async Task<IActionResult> Index()
    {
        var fases = await _partidoService.GetFases();
        var partidos = await _partidoService.GetAll();

        var viewModel = fases.Select(f => new ViewModels.Partidos.PartidoListViewModel
        {
            Fase = f,
            Partidos = partidos.Where(p => p.Fase == f).ToList()
        }).ToList();

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var partido = await _partidoService.GetById(id);
        if (partido == null) return NotFound();

        var puedeApostar = partido.Estado == "programado"
            && partido.ApuestasHabilitadas
            && partido.FechaHora > DateTime.UtcNow
            && HttpContext.Session.GetInt32("UserId") != null;

        var viewModel = new ViewModels.Partidos.PartidoDetalleViewModel
        {
            Partido = partido,
            Cuotas = partido.Cuotas.ToList(),
            PuedeApostar = puedeApostar
        };

        return View(viewModel);
    }
}
