using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ComentariosProyecto
    {
        public int Id { get; set; }

        [Required]
        public int ProyectoId { get; set; }

        public Proyecto Proyecto { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string NombreComentario { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Detalles { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
