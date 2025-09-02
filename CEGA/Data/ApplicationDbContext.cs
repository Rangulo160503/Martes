using CEGA.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CEGA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Puesto> Puestos { get; set; } = null!;
        public DbSet<Empleado> Empleados { get; set; } = null!;
        public DbSet<EmpleadosSalarios> EmpleadosSalarios { get; set; } = null!;

    }
}
