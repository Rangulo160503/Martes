using System.Collections.Generic;

namespace CEGA.Models
{
    public class BuscarUsuarioFiltroViewModel
    {
        public string? FiltroTexto { get; set; }
        public string? RolSeleccionado { get; set; }

        public List<ApplicationUser> UsuariosFiltrados { get; set; } = new();
        public List<string> RolesDisponibles { get; set; } = new();
    }
}
