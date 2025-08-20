using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class PoolCorreo
    {
        public int Id { get; set; }

        [Required] public string Nombre { get; set; } = null!;

        // En BD puede ser NULL
        public string? Descripcion { get; set; }

        // Requerido en BD, pero pueden venir varios correos separados por ';'
        [Required]
        public string Correos { get; set; } = null!;
    }

}

