using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class CampMarketing
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string AsuntoCorreo { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string NombrePool { get; set; } = string.Empty; // vínculo por nombre
    }
}
