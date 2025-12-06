using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("Incapacidad")]
    public class Incapacidad
    {
        // PK autoincremental
        public int Id { get; set; }

        // FK hacia Empleado.Cedula
        [ForeignKey(nameof(Empleado))]
        public int Cedula { get; set; }

        // Archivo binario
        public byte[] Archivo { get; set; } = default!;

        // Fecha de incapacidad / carga
        public DateTime Fecha { get; set; }

        // Navegación
        public Empleado? Empleado { get; set; }
    }

}
