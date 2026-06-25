using System.ComponentModel.DataAnnotations;

namespace sistemaapuestas.ViewModels.Billetera;

public class RetirarViewModel
{
    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "El monto mínimo es $0.01")]
    public decimal Monto { get; set; }

    public decimal SaldoActual { get; set; }
}
