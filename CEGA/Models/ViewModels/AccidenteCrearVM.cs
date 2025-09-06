using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class AccidenteCrearVM
    {
        [Required] public DateTime Fecha { get; set; } = DateTime.Now;
        public int? ProyectoId { get; set; }
        [Required] public int CedulaEmpleado { get; set; }

        // dropdowns
        public IEnumerable<SelectListItem> Proyectos { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Empleados { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
