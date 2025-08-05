using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string? Email { get; set; }
    }
}
