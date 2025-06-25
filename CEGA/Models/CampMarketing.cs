using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class CampMarketing
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public required string AsuntoCorreo { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public required string ImagenUrl { get; set; }
    }
}
