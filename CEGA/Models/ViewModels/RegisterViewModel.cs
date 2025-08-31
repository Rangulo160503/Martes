using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CEGA.Models.ViewModels
{
    public class RegisterViewModel
    {
        // ===== Credenciales =====
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = "";

        // ===== Empleado =====
        [Required, Range(100000000, 999999999, ErrorMessage = "Cédula debe tener 9 dígitos")]
        public int Cedula { get; set; }

        [Required] public string Nombre { get; set; } = "";
        public string? SegundoNombre { get; set; }
        [Required] public string Apellido1 { get; set; } = "";
        public string? Apellido2 { get; set; }

        [Required] public string TelefonoPersonal { get; set; } = "";    // 8 dígitos
        [Required] public string TelefonoEmergencia { get; set; } = "";  // 8 dígitos

        [Required] public string Sexo { get; set; } = "M";               // M/F/X/Prefiero no decir

        [Required, DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; }

        public string? TipoSangre { get; set; }
        public string? Alergias { get; set; }
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        public string? PolizaSeguro { get; set; }

        [Required]
        public int? PuestoId { get; set; }

        // UI
        public IEnumerable<SelectListItem> Puestos { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}