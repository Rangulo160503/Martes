namespace CEGA.Models.ViewModels
{
    public class UsuarioListaVM
    {
        public int Cedula { get; set; }
        // Nombre completo armado con Nombre + SegundoNombre + Apellido1 + Apellido2
        public string NombreCompleto { get; set; } = "";

        // Usuario de login
        public string Username { get; set; } = "";

        // Correo electrónico
        public string Email { get; set; } = "";

        // Teléfono personal
        public string? TelefonoPersonal { get; set; }

        // Rol en la aplicación (convertido desde tinyint a texto legible)
        public string Rol { get; set; } = "";

        // Estado activo/inactivo
        public bool Activo { get; set; }
    }
}
