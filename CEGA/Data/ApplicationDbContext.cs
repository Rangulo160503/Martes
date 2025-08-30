using CEGA.Models;
using CEGA.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TuProyecto.Models;


namespace CEGA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Puesto> Puestos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }

    }
}
