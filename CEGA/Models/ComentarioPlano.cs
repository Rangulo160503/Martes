using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ComentarioPlano
    {
        public int Id { get; set; }

        [Required]
        public int PlanoId { get; set; }

        public Plano Plano { get; set; }

        [Required(ErrorMessage = "El comentario no puede estar vacío")]
        public string Texto { get; set; }
    }
}
