using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class IncapacidadEmpleado
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioID { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo o descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe adjuntar un archivo")]
        public string ArchivoRuta { get; set; }  // Ruta donde se almacena el archivo en el servidor

        public DateTime FechaPresentacion { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "Pendiente";  // Pendiente, Aprobada, Rechazada
    }
}
