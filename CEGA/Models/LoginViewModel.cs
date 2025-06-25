using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        [RegularExpression(@"^[a-zA-Z0-9@._-]+$", ErrorMessage = "Caracteres inválidos en el correo")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[^\s<>]*$", ErrorMessage = "Caracteres inválidos en la contraseña")]
        public string? Password { get; set; }
    }
}
