using sistemaapuestas.Models;

namespace sistemaapuestas.ViewModels.Billetera;

public class BilleteraViewModel
{
    public decimal Saldo { get; set; }
    public string Moneda { get; set; } = "USD";
    public List<Movimiento> Movimientos { get; set; } = new();
}
