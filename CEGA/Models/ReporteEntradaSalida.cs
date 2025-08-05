using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ReporteEntradaSalida
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string NombreReporte { get; set; }

        [Required(ErrorMessage = "No se seleccionó grupo de usuarios")]
        public string RolSeleccionado { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "No se seleccionó movimiento")]
        public string Movimiento { get; set; } // Entrada o Salida

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
