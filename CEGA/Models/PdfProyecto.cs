using CEGA.Models.Seguimiento;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("PdfProyecto")]
    [PrimaryKey(nameof(IdPdf), nameof(IdProyecto))] // EF Core 7+
    public class PdfProyecto
    {
        public int IdPdf { get; set; }
        [ForeignKey(nameof(IdPdf))] public Pdf Pdf { get; set; } = default!;

        public int IdProyecto { get; set; }
        [ForeignKey(nameof(IdProyecto))] public Proyecto Proyecto { get; set; } = default!;
    }
}
