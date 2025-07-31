namespace CEGA.Models.DTOs
{
    public class ReporteProyectoDTO
    {
        public int ProyectoId { get; set; }
        public string NombreProyecto { get; set; }
        public int TotalTareas { get; set; }
        public int TareasAsignadas { get; set; }
        public int TareasSinAsignar => TotalTareas - TareasAsignadas;
    }
}
