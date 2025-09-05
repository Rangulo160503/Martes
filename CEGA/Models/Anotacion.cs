using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("Anotaciones")]
    public class Anotacion
    {
        [Key] public int IdAnotacion { get; set; }

        [Required] public int IdPdf { get; set; }
        [ForeignKey("IdPdf")] public Pdf Pdf { get; set; } = default!;

        [Required] public int Cedula { get; set; }
        [ForeignKey("Cedula")] public Empleado Empleado { get; set; } = default!;

        [Required] public string Texto { get; set; } = string.Empty;
    }
}
