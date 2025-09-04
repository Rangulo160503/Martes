using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class PoolCorreo
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        [Required] // emails separados por ';'
        public string Correos { get; set; } = string.Empty;

        public byte[]? Archivo { get; set; } // opcional
    }
}
