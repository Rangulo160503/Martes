using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("VacacionesEmpleado")]
    public class VacacionesEmpleado
    {
        [Key]
        public int Id { get; set; }                // PK simple

        [Required]
        public int Cedula { get; set; }            // FK a Empleado(Cedula)

        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }        // Día tomado

        [ForeignKey(nameof(Cedula))]
        public Empleado? Empleado { get; set; }    // Navegación (opcional)
    }
}
