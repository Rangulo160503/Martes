using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models.Domain
{
    [Table("PUESTO")]
    public class Puesto
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Nombre { get; set; } = "";
        [MaxLength(300)] public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
