using Microsoft.AspNetCore.Identity;

namespace CEGA.Models.Identity
{
    // Mínimo: sin datos personales (van en EMPLEADO)
    public class ApplicationUser : IdentityUser
    {
        // Si luego necesitas algo más (claims, flags), lo agregamos aquí.
    }
}
