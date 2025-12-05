using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    public class Incapacidad
    {
        public int Id { get; set; }

        // 🔹 Esta columna EXISTE en la tabla dbo.Incapacidad
        [ForeignKey(nameof(Empleado))]      // <- le decimos a EF: este es el FK
        public int Cedula { get; set; }

        public byte[] Archivo { get; set; } = default!;

        public DateTime Fecha { get; set; }

        // 🔹 Navegación al empleado (PK = Cedula)
        public Empleado? Empleado { get; set; }
    }
}
