namespace CEGA.Models.Seguimiento
{
    public class TareaEditarInlinePost
    {
        public int Id { get; set; }
        public int? ProyectoId { get; set; }
        public int CedulaEmpleado { get; set; }
        public string Titulo { get; set; } = "";
        public string? Comentario { get; set; }
        public int? ReturnProyectoId { get; set; }
    }
}
