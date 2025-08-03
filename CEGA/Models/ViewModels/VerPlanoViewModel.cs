namespace CEGA.Models.ViewModels
{
    public class VerPlanoViewModel
    {
        public Plano Plano { get; set; }
        public List<ComentarioPlano>? Comentarios { get; set; }
        public List<TareaPlano> Tareas { get; set; }
    }

}
