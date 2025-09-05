using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class PdfUploadVM
    {
        [Required(ErrorMessage = "La cédula es obligatoria.")]
        public int Cedula { get; set; }

        [Required(ErrorMessage = "Selecciona un archivo PDF.")]
        public IFormFile? PdfFile { get; set; }
    }
}
