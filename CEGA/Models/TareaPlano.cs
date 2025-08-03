using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class TareaPlano
    {
        public int Id { get; set; }

        [Required]
        public int PlanoId { get; set; }
        public Plano Plano { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe ingresar una fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Debe indicar un responsable")]
        public string Responsable { get; set; }

        public float CoordenadaX { get; set; }
        public float CoordenadaY { get; set; }
    }
}
