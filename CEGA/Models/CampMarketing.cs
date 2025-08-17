using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class CampMarketing
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string AsuntoCorreo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string ImagenUrl { get; set; } = string.Empty;
    }
}
