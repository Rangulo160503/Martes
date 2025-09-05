using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models.Seguimiento
{
    [Table("Tareas")]
    public class Tarea
    {
        public int Id { get; set; }
        public int? ProyectoId { get; set; } // aún no usamos proyecto

        [Required]                    // <- quitar StringLength aquí
        public int CedulaEmpleado { get; set; }

        [Required, StringLength(120)]
        public string Titulo { get; set; } = "";

        [StringLength(1000)]
        public string? ComentarioInicial { get; set; }
    }
}
