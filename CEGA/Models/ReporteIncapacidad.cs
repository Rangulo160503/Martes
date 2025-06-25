using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ReporteIncapacidad
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string NombreReporte { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string RolSeleccionado { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
