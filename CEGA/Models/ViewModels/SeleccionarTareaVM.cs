using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class SeleccionarTareaVM
    {
        [Required(ErrorMessage = "Selecciona una tarea")]
        [Display(Name = "Tarea")]
        public int TareaId { get; set; }
    }
}
