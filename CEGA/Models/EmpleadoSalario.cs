using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class EmpleadoSalario
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "El salario es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario no puede ser negativo")]
        public decimal SalarioMensual { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
