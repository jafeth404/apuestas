using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("billeteras")]
public class Billetera
{
    [Key]
    [Column("id_billetera")]
    public int Id { get; set; }

    [Column("id_usuario")]
    [Required]
    public int UsuarioId { get; set; }

    [Column("saldo")]
    public decimal Saldo { get; set; } = 0.00m;

    [Column("moneda")]
    [Required, MaxLength(10)]
    public string Moneda { get; set; } = "USD";

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }

    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
