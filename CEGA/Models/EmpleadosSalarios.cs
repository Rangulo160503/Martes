using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("EmpleadosSalarios")]
    public class EmpleadosSalarios
    {
        [Key] public int Id { get; set; }

        [Required] public int Cedula { get; set; }                     // FK → Empleado
        [ForeignKey(nameof(Cedula))] public Empleado? Empleado { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999)] public decimal Monto { get; set; }

        [MaxLength(10)] public string Moneda { get; set; } = "CRC";

        [DataType(DataType.Date)] public DateTime FechaInicio { get; set; } = DateTime.Today;
        [DataType(DataType.Date)] public DateTime? FechaFin { get; set; }

        [MaxLength(200)] public string? Notas { get; set; }
    }
}
