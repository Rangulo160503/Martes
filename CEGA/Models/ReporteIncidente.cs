using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ReporteIncidente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "Nombre del Reporte")]
        public string NombreReporte { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [Display(Name = "ID del Usuario")]
        public string UsuarioID { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del Accidente")]
        public DateTime FechaAccidente { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [StringLength(2000, ErrorMessage = "Máximo 2000 caracteres")]
        [Display(Name = "Descripción del Accidente")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [RegularExpression("Sí|No", ErrorMessage = "Debe seleccionar Sí o No")]
        [Display(Name = "¿Hubo Incapacidad?")]
        public string Incapacidad { get; set; } // "Sí" o "No"

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
