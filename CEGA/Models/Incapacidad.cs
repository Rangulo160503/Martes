using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("Incapacidad")]
    public class Incapacidad
    {
        [Required]
        [Key]
        public int Cedula { get; set; }
        [Required]
        public byte[] Archivo { get; set; } = Array.Empty<byte>();

        // Navegación
        [ForeignKey(nameof(Cedula))]
        public Empleado? Empleado { get; set; }

        // La BD le pone el valor por defecto (GETDATE()/SYSUTCDATETIME)
    }
}
