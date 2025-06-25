using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Utilice el formato xxxx-xxxx")]
        public string? Telefono { get; set; }
    }
}
