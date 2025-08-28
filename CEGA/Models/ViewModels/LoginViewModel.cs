using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Usuario o Email es requerido")]
        [Display(Name = "Usuario o Email")]
        public string UserOrEmail { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = "";

        public string? ReturnUrl { get; set; }
    }
}
