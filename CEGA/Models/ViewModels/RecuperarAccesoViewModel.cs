using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class RecuperarAccesoViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";
    }
}
