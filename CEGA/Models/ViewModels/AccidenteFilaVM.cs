namespace CEGA.Models.ViewModels
{
    public class AccidenteFilaVM
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int? ProyectoId { get; set; }
        public string? ProyectoNombre { get; set; }
        public int CedulaEmpleado { get; set; }
        public string? EmpleadoNombre { get; set; }
    }
}
