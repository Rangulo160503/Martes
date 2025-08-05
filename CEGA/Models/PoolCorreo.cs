using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class PoolCorreo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico incorrecto")]
        public string Correos { get; set; }

    }
}
