using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemaapuestas.Models;

[Table("administradores")]
public class Administrador
{
    [Key]
    [Column("id_admin")]
    public int Id { get; set; }

    [Column("nombre")]
    [Required, MaxLength(100)]
    public string Nombre { get; set; }

    [Column("usuario")]
    [Required, MaxLength(50)]
    public string Usuario { get; set; }

    [Column("contrasena")]
    [Required, MaxLength(255)]
    public string Contrasena { get; set; }

    [Column("rol")]
    [Required, MaxLength(50)]
    public string Rol { get; set; }
}
