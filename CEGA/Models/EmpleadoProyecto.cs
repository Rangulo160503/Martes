namespace CEGA.Models
{
    // Models/EmpleadoProyecto.cs
    public class EmpleadoProyecto
    {
        public int Cedula { get; set; }
        public int ProyectoId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public Empleado? Empleado { get; set; }
    }

}
