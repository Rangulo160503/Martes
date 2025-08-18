using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class Proyecto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public string Detalles { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
        public ICollection<TareasProyecto> Tareas { get; set; } = new List<TareasProyecto>();
        public ICollection<ComentariosProyecto> Comentarios { get; set; } = new List<ComentariosProyecto>();
    }
}
