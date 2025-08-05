using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ClienteMarketing
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico incorrecto")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Telefono { get; set; }
    }
}
