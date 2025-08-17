namespace CEGA.Models.ViewModels
{
    public class MarketingPageVM
    {
        // Listados
        public List<CampMarketing> Campanias { get; set; } = new();
        public List<PoolCorreo> Pools { get; set; } = new();
        public List<ProgramacionDistribucion> Programaciones { get; set; } = new();
        public List<ClienteMarketing> Clientes { get; set; } = new();

        // Formularios (opcional, si ocupas prellenar)
        public CampMarketing CampaniaForm { get; set; } = new();
        public PoolCorreo PoolForm { get; set; } = new();
        public ProgramacionDistribucion ProgramacionForm { get; set; } = new();
        public ClienteMarketing ClienteForm { get; set; } = new();

        // Resultados de búsquedas
        public List<CampMarketing>? ResultadoCampanias { get; set; }
        public PoolCorreo? PoolEncontrado { get; set; }
        public ProgramacionDistribucion? ProgramacionEncontrada { get; set; }
        public ClienteMarketing? ClienteEncontrado { get; set; }
    }
}
