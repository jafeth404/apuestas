using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public interface IUsuarioService
{
    Task<Usuario> Registrar(string nombre, string correo, string contrasena, DateTime fechaNacimiento);
    Task<Usuario?> Login(string correo, string contrasena);
    Task<Usuario?> GetById(int id);
    Task<bool> CorreoExiste(string correo);
}
