using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class TareaCrearVM
    {
        [Required, StringLength(120)]
        [Display(Name = "Título de la tarea")]
        public string Titulo { get; set; } = "";

        [Required]                  // <- quitar StringLength aquí
        [Display(Name = "Empleado asignado")]
        public int CedulaEmpleado { get; set; }

        [StringLength(1000)]
        [Display(Name = "Comentario inicial")]
        public string? Comentario { get; set; }

        public IEnumerable<SelectListItem> Empleados { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
