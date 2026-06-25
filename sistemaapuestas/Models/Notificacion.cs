using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("notificaciones")]
public class Notificacion
{
    [Key]
    [Column("id_notificacion")]
    public int Id { get; set; }

    [Column("id_usuario")]
    [Required]
    public int UsuarioId { get; set; }

    [Column("mensaje")]
    [Required]
    public string Mensaje { get; set; }

    [Column("tipo")]
    [MaxLength(30)]
    public string? Tipo { get; set; }

    [Column("leida")]
    public bool Leida { get; set; } = false;

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }
}
