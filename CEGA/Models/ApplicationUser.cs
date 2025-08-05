using Microsoft.AspNetCore.Identity;

namespace CEGA.Models
{


    public class ApplicationUser : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? SubRol { get; set; }

    }


}
