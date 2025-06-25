using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class ProgramacionDistribucion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DataType(DataType.DateTime)]
        public DateTime FechaHoraEnvio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string NombrePool { get; set; }  // Se validará si existe
    }
}

