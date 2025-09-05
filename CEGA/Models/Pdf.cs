using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("Pdf")]
    public class Pdf
    {
        [Key] public int IdPdf { get; set; }

        [Required] public int Cedula { get; set; }   // FK a Empleado

        [Required] public byte[] PdfArchivo { get; set; } = default!;
    }
}
