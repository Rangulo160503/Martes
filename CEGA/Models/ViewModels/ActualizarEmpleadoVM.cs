namespace CEGA.Models.ViewModels
{
    public class ActualizarEmpleadoVM
    {
        public string Id { get; set; } = "";
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SubRol { get; set; }
        // Opcionales (normalmente no cambiar):
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
