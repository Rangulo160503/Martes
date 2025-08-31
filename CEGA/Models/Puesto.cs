using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    // La tabla real es [dbo].[Puesto]
    [Table("Puesto")]
    public class Puesto
    {
        [Key]
        public int Id { get; set; }

        // Campos existentes en la tabla
        [MaxLength(20)]
        public string? Codigo { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = "";

        public bool Activo { get; set; } = true;

        [MaxLength(200)]
        public string? Departamento { get; set; }

        // En tu SELECT aparecen en este orden:
        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [MaxLength(500)]
        public string? Requisitos { get; set; }

        [MaxLength(100)]
        public string? Nivel { get; set; }

        // Si en SQL es decimal/money, deja decimal en C#
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalarioBase { get; set; }

        [MaxLength(50)]
        public string? Moneda { get; set; } = "CRC";

        [MaxLength(100)]
        public string? Jornada { get; set; }
    }
}
