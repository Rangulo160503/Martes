using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        [RegularExpression(@"^[a-zA-Z0-9@._-]+$", ErrorMessage = "Caracteres inválidos en el correo")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Use el formato xxxx-xxxx")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[^\s<>]*$", ErrorMessage = "Caracteres inválidos en la contraseña")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }
    }
}
