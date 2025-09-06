using CEGA.Models.Seguimiento;

namespace CEGA.Models.ViewModels
{
    public class ProyectoAsignarVM
    {
        public int IdProyecto { get; set; }
        public string NombreProyecto { get; set; } = "";

        // Para crear una nueva tarea DIRECTA al proyecto
        public TareaCrearVM NuevaTarea { get; set; } = new TareaCrearVM();

        // Listas para mostrar en la vista
        public IEnumerable<Tarea> TareasDelProyecto { get; set; } = Enumerable.Empty<Tarea>();
        public IEnumerable<Tarea> TareasSinProyecto { get; set; } = Enumerable.Empty<Tarea>();
    }
}
