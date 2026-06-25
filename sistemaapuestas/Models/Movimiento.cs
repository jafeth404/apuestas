using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("movimientos")]
public class Movimiento
{
    [Key]
    [Column("id_movimiento")]
    public int Id { get; set; }

    [Column("id_billetera")]
    [Required]
    public int BilleteraId { get; set; }

    [Column("tipo")]
    [Required, MaxLength(20)]
    public string Tipo { get; set; }

    [Column("monto")]
    public decimal Monto { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(BilleteraId))]
    public Billetera Billetera { get; set; }
}
