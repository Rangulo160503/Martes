using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("Descarga")]
    public class Descarga
    {
        [Key] public int IdDescarga { get; set; }

        [Required] public int IdPdf { get; set; }
        [ForeignKey("IdPdf")] public Pdf Pdf { get; set; } = default!;

        public bool IncluirAnotaciones { get; set; } = true;
        public bool IncluirProyectos { get; set; } = true;
        public bool IncluirTareas { get; set; } = true;
    }
}
