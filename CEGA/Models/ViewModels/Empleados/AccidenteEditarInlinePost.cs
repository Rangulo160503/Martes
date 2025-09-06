namespace CEGA.Models.ViewModels.Empleados
{
    public class AccidenteEditarInlinePost
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int? ProyectoId { get; set; }
        public int CedulaEmpleado { get; set; }

        public int? ReturnProyectoId { get; set; }
        public int? ReturnCedula { get; set; }
        public string? Desde { get; set; }
        public string? Hasta { get; set; }
    }
}
