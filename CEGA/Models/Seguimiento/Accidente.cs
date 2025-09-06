using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Models.Seguimiento
{
    [Table("Accidentes")]
    [Index(nameof(ProyectoId))]
    [Index(nameof(CedulaEmpleado))]
    [Index(nameof(Fecha))]
    public class Accidente
    {
        [Key] public int Id { get; set; }

        public int? ProyectoId { get; set; }               // FK → Proyecto.IdProyecto
        [Required] public int CedulaEmpleado { get; set; } // FK → Empleado.Cedula

        [Required] public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
