namespace CEGA.Models.ViewModels
{
    public class ProyectoPageVM
    {
        public List<Proyecto> Proyectos { get; set; } = new();
        public List<TareasProyecto> Tareas { get; set; } = new();
        public List<ComentariosProyecto> Comentarios { get; set; } = new();
    }

}
