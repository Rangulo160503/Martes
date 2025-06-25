using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ReporteIncidente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string NombreReporte { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string UsuarioID { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaAccidente { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "No se seleccionó incapacidad")]
        public string Incapacidad { get; set; } // "Sí" o "No"

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
