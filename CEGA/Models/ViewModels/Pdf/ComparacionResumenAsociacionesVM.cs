namespace CEGA.Models.ViewModels.Pdf
{
    public class ComparacionResumenAsociacionesVM
    {
        public ResumenAsociacionesVM Izquierdo { get; set; } = null!;
        public ResumenAsociacionesVM Derecho { get; set; } = null!;

        // Utilidades precalculadas (opcional, puedes calcular en la vista si prefieres)
        public IReadOnlyCollection<string> ProyectosSoloIzq { get; set; } = Array.Empty<string>();
        public IReadOnlyCollection<string> ProyectosSoloDer { get; set; } = Array.Empty<string>();
        public IReadOnlyCollection<string> TareasSoloIzq { get; set; } = Array.Empty<string>();
        public IReadOnlyCollection<string> TareasSoloDer { get; set; } = Array.Empty<string>();
        public IReadOnlyCollection<string> AnotsSoloIzq { get; set; } = Array.Empty<string>();
        public IReadOnlyCollection<string> AnotsSoloDer { get; set; } = Array.Empty<string>();
    }
}
