using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class VacacionesEmpleado
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioID { get; set; }

        [Required(ErrorMessage = "Fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Cantidad de días es obligatoria")]
        [Range(1, 30, ErrorMessage = "Los días deben ser entre 1 y 30")]
        public int DiasSolicitados { get; set; }

        public int DiasDisponibles { get; set; } = 15;

        public string Estado { get; set; } = "Pendiente"; // Aprobado, Rechazado, Extensión

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
    }
}
