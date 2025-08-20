using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class CampMarketing
    {
        public int Id { get; set; }

        [Required] public string Nombre { get; set; } = null!;
        [Required] public string AsuntoCorreo { get; set; } = null!;
        [Required] public string Descripcion { get; set; } = null!;

        // En BD permiten NULL → deben ser nullable aquí
        public string? ImagenUrl { get; set; }
        public string? NombrePool { get; set; }
    }
}
