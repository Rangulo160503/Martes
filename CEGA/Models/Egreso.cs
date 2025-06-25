using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class Egreso
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Monto { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(255)]
        public string Concepto { get; set; }
    }


}
