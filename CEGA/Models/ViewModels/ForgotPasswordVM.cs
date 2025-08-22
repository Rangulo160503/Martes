using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";
    }
}
