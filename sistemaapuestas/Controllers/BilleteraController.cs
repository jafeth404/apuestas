using Microsoft.AspNetCore.Mvc;
using sistemaapuestas.Filters;
using sistemaapuestas.Services;
using sistemaapuestas.ViewModels.Billetera;

namespace sistemaapuestas.Controllers;

[RequireAuth]
public class BilleteraController : Controller
{
    private readonly IBilleteraService _billeteraService;
    private readonly INotificacionService _notificacionService;

    public BilleteraController(IBilleteraService billeteraService, INotificacionService notificacionService)
    {
        _billeteraService = billeteraService;
        _notificacionService = notificacionService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        var billetera = await _billeteraService.GetByUsuario(userId);
        var movimientos = await _billeteraService.GetMovimientos(userId);

        var viewModel = new BilleteraViewModel
        {
            Saldo = billetera?.Saldo ?? 0,
            Moneda = billetera?.Moneda ?? "USD",
            Movimientos = movimientos
        };

        return View(viewModel);
    }

    public IActionResult Depositar()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Depositar(DepositarViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        if (!ModelState.IsValid)
            return View(model);

        await _billeteraService.Depositar(userId, model.Monto);

        await _notificacionService.Crear(
            userId,
            $"Depósito de ${model.Monto:F2} realizado exitosamente",
            "deposito");

        TempData["Success"] = "Depósito realizado con éxito";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Retirar()
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var saldo = await _billeteraService.GetSaldo(userId);
        return View(new RetirarViewModel { SaldoActual = saldo });
    }

    [HttpPost]
    public async Task<IActionResult> Retirar(RetirarViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        if (!ModelState.IsValid)
            return View(model);

        var exito = await _billeteraService.Retirar(userId, model.Monto);
        if (!exito)
        {
            ModelState.AddModelError("Monto", "Saldo insuficiente");
            model.SaldoActual = await _billeteraService.GetSaldo(userId);
            return View(model);
        }

        await _notificacionService.Crear(
            userId,
            $"Retiro de ${model.Monto:F2} procesado",
            "retiro");

        TempData["Success"] = "Retiro procesado con éxito";
        return RedirectToAction("Index");
    }
}
