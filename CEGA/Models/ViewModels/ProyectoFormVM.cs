using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class ProyectoFormVM
    {
        public int? IdProyecto { get; set; }

        [Required, StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        public string FormAction { get; set; } = "/Seguimiento/CrearProyecto";
        public string SubmitText { get; set; } = "Crear";
    }
}
