using Microsoft.AspNetCore.Mvc.Rendering;

namespace CEGA.Models.ViewModels.Pdf
{
    public class AnotacionFormVM
    {
        public int? IdAnotacion { get; set; }
        public int IdPdf { get; set; }
        public int Cedula { get; set; }
        public string Texto { get; set; } = string.Empty;

        // Para la vista
        public string FormAction { get; set; } = "/Pdf/CrearAnotacion";
        public string SubmitText { get; set; } = "Guardar";
        public IEnumerable<SelectListItem> Empleados { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
