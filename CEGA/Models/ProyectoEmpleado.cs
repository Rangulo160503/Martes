using CEGA.Models.Seguimiento;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("ProyectoEmpleado")]
    [PrimaryKey(nameof(IdProyecto), nameof(Cedula))] // EF Core 7+
    public class ProyectoEmpleado
    {
        public int IdProyecto { get; set; }
        [ForeignKey("IdProyecto")] public Proyecto Proyecto { get; set; } = default!;

        public int Cedula { get; set; }
        [ForeignKey("Cedula")] public Empleado Empleado { get; set; } = default!;
    }
}
