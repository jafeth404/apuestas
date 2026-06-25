using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("usuarios")]
public class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int Id { get; set; }

    [Column("nombre")]
    [Required, MaxLength(100)]
    public string Nombre { get; set; }

    [Column("correo")]
    [Required, MaxLength(150)]
    public string Correo { get; set; }

    [Column("contrasena")]
    [Required, MaxLength(255)]
    public string Contrasena { get; set; }

    [Column("fecha_nacimiento")]
    [Required]
    public DateTime FechaNacimiento { get; set; }

    [Column("estado")]
    [Required, MaxLength(20)]
    public string Estado { get; set; } = "activo";

    [Column("fecha_registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public Billetera? Billetera { get; set; }
    public ICollection<Apuesta> Apuestas { get; set; } = new List<Apuesta>();
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
}
