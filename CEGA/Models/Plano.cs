using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class Plano
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una disciplina")]
        public string Disciplina { get; set; }

        public string? NombreArchivo { get; set; }
        public DateTime FechaSubida { get; set; } = DateTime.Now;
        public string? SubidoPor { get; set; }
        public byte[]? Archivo { get; set; }
    }

}
