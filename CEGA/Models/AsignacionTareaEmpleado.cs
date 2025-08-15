using CEGA.Models;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class AsignacionTareaEmpleado
    {
        public int Id { get; set; }

        [Required]
        public int TareaId { get; set; }
        public TareaProyecto Tarea { get; set; }
        [Required(ErrorMessage = "Se necesita seleccionar un empleado")]
        public string UsuarioId { get; set; }
        public ApplicationUser Usuario { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    }
}
