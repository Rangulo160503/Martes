namespace CEGA.Models.Domain
{
    public class Puesto
    {
        public int Id { get; set; }                 // PK (IDENTITY)
        public string Nombre { get; set; } = "";    // UNIQUE
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
