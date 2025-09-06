using CEGA.Models.Seguimiento;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("PdfTarea")]
    [PrimaryKey(nameof(IdPdf), nameof(IdTarea))] // EF Core 7+
    public class PdfTarea
    {
        public int IdPdf { get; set; }
        [ForeignKey(nameof(IdPdf))] public Pdf Pdf { get; set; } = default!;

        public int IdTarea { get; set; } // ← referencia a dbo.Tareas(Id)
        [ForeignKey(nameof(IdTarea))] public Tarea Tarea { get; set; } = default!;
    }
}
