using Microsoft.AspNetCore.Mvc.Rendering;

namespace CEGA.Models.ViewModels
{
    public class TareasPageVM
    {
        public List<CEGA.Models.TareasProyecto> Tareas { get; set; } = new();
        public List<CEGA.Models.TareasProyecto> TareasSinAsignar { get; set; } = new();
        public List<EmpleadoTareasVM> TareasPorEmpleado { get; set; } = new();

        // <-- NUEVO: lo que envías desde Index() (proyección desde TareaPlano)
        public List<AsignacionTareaPlanoVM> Asignaciones { get; set; } = new();

        // <-- NUEVO: si ya añadiste la pestaña de comentarios
        public List<CEGA.Models.ComentariosProyecto> Comentarios { get; set; } = new();

        // Totales usados en la vista (si los necesitas)
        public int TotalTareas => Tareas?.Count ?? 0;
        public int TotalSinAsignar => TareasSinAsignar?.Count ?? 0;
        public int TotalAsignaciones => Asignaciones?.Count ?? 0;
        public int TotalComentarios => Comentarios?.Count ?? 0;
    }

    public class EmpleadoTareasVM
    {
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public List<CEGA.Models.TareasProyecto> Tareas { get; set; } = new();
        public int Cantidad => Tareas?.Count ?? 0;
    }

}
