using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class PuestoCreateVM
    {
        // Identificación del puesto
        [Required, StringLength(20)]
        public string Codigo { get; set; } = "";     // Ej: "ADM-001"

        [Required, StringLength(100)]
        public string Nombre { get; set; } = "";     // Ej: "Administrador de Sistemas"

        [StringLength(200)]
        public string? Departamento { get; set; }    // Ej: "Tecnología"

        // Características
        [Required, StringLength(500)]
        public string Descripcion { get; set; } = ""; // Breve resumen de responsabilidades

        [StringLength(500)]
        public string? Requisitos { get; set; }       // Ej: "Bachillerato en informática, 2 años experiencia"

        [StringLength(100)]
        public string? Nivel { get; set; }            // Ej: "Junior", "Senior", "Gerencial"

        // Condiciones laborales
        [Range(0, double.MaxValue)]
        public decimal? SalarioBase { get; set; }

        [StringLength(50)]
        public string? Moneda { get; set; } = "CRC"; // CRC/USD

        [StringLength(100)]
        public string? Jornada { get; set; }          // Ej: "Tiempo completo", "Medio tiempo", "Nocturno"

        public bool Activo { get; set; } = true;
    }
}
