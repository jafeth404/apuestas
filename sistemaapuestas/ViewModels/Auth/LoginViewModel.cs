using System.ComponentModel.DataAnnotations;

namespace sistemaapuestas.ViewModels.Auth;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress]
    public string Correo { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; }
}
