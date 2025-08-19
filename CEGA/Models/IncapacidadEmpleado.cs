using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class IncapacidadEmpleado
    {
        public int Id { get; set; }
        public string UsuarioID { get; set; } = "";
        public string Descripcion { get; set; } = "";

        // Opción A: guardar en BD
        public byte[]? ArchivoContenido { get; set; }
        public string? ArchivoNombre { get; set; }
        public string? ArchivoTipo { get; set; }
        public long? ArchivoTamano { get; set; }
    }
}
