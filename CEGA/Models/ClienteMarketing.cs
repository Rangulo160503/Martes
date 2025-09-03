using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ClienteMarketing
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(256)]
        public string Correo { get; set; } = string.Empty;
    }
}
