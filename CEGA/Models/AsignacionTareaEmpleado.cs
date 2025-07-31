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
        public string UsuarioId { get; set; } // Suponiendo que usás Identity

        public ApplicationUser Usuario { get; set; } // Relación con tabla de usuarios
    }
}
