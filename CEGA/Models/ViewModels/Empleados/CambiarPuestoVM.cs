using Microsoft.AspNetCore.Mvc.Rendering;

namespace CEGA.Models.ViewModels.Empleados
{
    public class CambiarPuestoVM
    {
        public int Cedula { get; set; }                 // empleado seleccionado
        public int PuestoId { get; set; }               // nuevo puesto

        public IEnumerable<SelectListItem> Empleados { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Puestos { get; set; } = new List<SelectListItem>();
    }
}
