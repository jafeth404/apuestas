using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using sistemaapuestas.Hubs;
using sistemaapuestas.Services;

namespace sistemaapuestas.Controllers;

public class PartidosController : Controller
{
    private readonly IPartidoService _partidoService;
    private readonly IWorldCupApiService _apiService;
    private readonly IHubContext<MatchHub> _hubContext;

    public PartidosController(IPartidoService partidoService, IWorldCupApiService apiService, IHubContext<MatchHub> hubContext)
    {
        _partidoService = partidoService;
        _apiService = apiService;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        await _apiService.RefreshMatches();
        await _hubContext.Clients.All.SendAsync("MatchesUpdated");

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
