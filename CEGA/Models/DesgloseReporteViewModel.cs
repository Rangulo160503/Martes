using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class DesgloseReporteViewModel
    {
        [Required(ErrorMessage = "El nombre del reporte es obligatorio.")]
        public string NombreReporte { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una categoría.")]
        public bool IncluirIngresos { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una categoría.")]
        public bool IncluirEgresos { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una categoría.")]
        public bool IncluirSalarios { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }
    }
}
