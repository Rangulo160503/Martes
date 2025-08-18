namespace CEGA.Models.ViewModels
{
    public class AsignacionTareaPlanoVM
    {
        // Asignación
        public int AsignacionId { get; set; }
        public string UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }

        // TareaPlano (FK = AsignacionesTareaEmpleado.TareaId)
        public int? TareaPlanoId { get; set; }
        public string? TareaDescripcion { get; set; }
        public DateTime? TareaFecha { get; set; }
        public string? TareaResponsable { get; set; }

        // Plano asociado
        public int? PlanoId { get; set; }
        public string? PlanoNombreArchivo { get; set; }
        public string? TareaTitulo => TareaDescripcion;
    }
}
