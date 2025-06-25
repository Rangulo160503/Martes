using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class AsignarRolViewModel
    {
        public string? UsuarioId { get; set; }

        public string? Email { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public string? RolSeleccionado { get; set; }

        public List<string> RolesDisponibles { get; set; } = new();
        public string? SubRol { get; set; }

        public List<string> SubRolesDisponibles { get; set; } = new();

    }
}
