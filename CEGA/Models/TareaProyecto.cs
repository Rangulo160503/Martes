using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class TareaProyecto
    {
        public int Id { get; set; }

        [Required]
        public int ProyectoId { get; set; }

        public Proyecto ?Proyecto { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string NombreTarea { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Detalles { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
    }
}
