using System;
using System.ComponentModel.DataAnnotations;

namespace TuProyecto.Models.ViewModels
{
    public class ReporteIncapacidadViewModel
    {
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
    }
}
