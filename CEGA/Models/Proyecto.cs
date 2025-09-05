using CEGA.Models.Seguimiento;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("Proyecto")]
    public class Proyecto
    {
        [Key] public int IdProyecto { get; set; }

        [Required, StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<ProyectoEmpleado> ProyectoEmpleados { get; set; } = new List<ProyectoEmpleado>();
        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}
