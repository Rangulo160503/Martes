using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class PuestoEmpleado
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioID { get; set; }

        [Required(ErrorMessage = "Debe ingresar un puesto")]
        public string Puesto { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor o igual a cero")]
        public decimal SalarioAsignado { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    }
}
