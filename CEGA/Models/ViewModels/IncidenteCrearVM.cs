using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class IncidentesCrearVM
    {
        [Required]
        public int ProyectoId { get; set; }
        public string ProyectoNombre { get; set; } = "";

        [Required, StringLength(120)]
        public string Titulo { get; set; } = "";

        [Required, StringLength(2000)]
        public string Descripcion { get; set; } = "";

        [Required]
        public string Severidad { get; set; } = "Media"; // Baja|Media|Alta

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [StringLength(120)]
        public string? ReportadoPor { get; set; }
    }
}
