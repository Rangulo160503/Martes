using System.Collections.Generic;

namespace CEGA.Models.ViewModels
{
    public class MarketingPageVM
    {
        public IEnumerable<CEGA.Models.ClienteMarketing> Clientes { get; set; }
            = System.Linq.Enumerable.Empty<CEGA.Models.ClienteMarketing>();

        public IEnumerable<CEGA.Models.PoolCorreo> Pools { get; set; }
            = System.Linq.Enumerable.Empty<CEGA.Models.PoolCorreo>();
        public IEnumerable<CEGA.Models.CampMarketing> Campanias { get; set; }
    = System.Linq.Enumerable.Empty<CEGA.Models.CampMarketing>();
    }
}
