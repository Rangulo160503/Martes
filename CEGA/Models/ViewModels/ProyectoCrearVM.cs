using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class ProyectoCrearVM
    {
        [Required, StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
    }
}
