using CEGA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    public class AsignacionesTareaEmpleado
    {
        public int Id { get; set; }

        [Required]
        public int TareaId { get; set; }
        public TareasProyecto Tarea { get; set; }
        [Required(ErrorMessage = "Se necesita seleccionar un empleado")]
        public string UsuarioId { get; set; }
        public ApplicationUser Usuario { get; set; }
        [NotMapped] public string? Comentario { get; set; }
        [NotMapped] public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    }
}
