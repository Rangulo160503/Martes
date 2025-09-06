namespace CEGA.Models
{
    public class CategoriaMovimiento
    {
        public int CategoriaId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// "I" = Ingreso, "G" = Gasto
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Si es un movimiento fijo (ejemplo: Datafonos)
        /// </summary>
        public bool EsFijo { get; set; }

        public bool Activo { get; set; }

        public DateTime CreadoEn { get; set; }
    }
}
