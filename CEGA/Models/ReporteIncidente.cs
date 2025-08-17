using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ReporteIncidente
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string NombreReporte { get; set; } = string.Empty;

        [StringLength(100)]
        public string? UsuarioID { get; set; }   // <- CAMBIO

        [Required]
        public DateTime FechaAccidente { get; set; } = DateTime.Today;

        [Required, StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required, StringLength(10)]
        public string Incapacidad { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
