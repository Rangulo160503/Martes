using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class PdfUploadVM
    {
        [Required(ErrorMessage = "Selecciona un archivo PDF.")]
        public IFormFile? PdfFile { get; set; }
    }
}
