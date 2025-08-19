using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class CrearEmpleadoVM
    {
        [Required] public string Nombre { get; set; } = "";
        [Required] public string Apellido { get; set; } = "";

        [Required, EmailAddress] public string Email { get; set; } = "";
        [Phone] public string? PhoneNumber { get; set; }

        // Si no lo llenas, usaré el Email como UserName
        public string? UserName { get; set; }

        public string? SubRol { get; set; } = "Empleado";

        [Required, DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = "";

        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = "";

        // Opcional: crear registro en EmpleadosSalarios
        [Range(0, double.MaxValue, ErrorMessage = "El salario no puede ser negativo")]
        public decimal? SalarioMensual { get; set; }
    }
}
