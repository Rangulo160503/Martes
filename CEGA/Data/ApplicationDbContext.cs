using CEGA.Models;
using CEGA.Models.Seguimiento;
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
        public DbSet<Incapacidad> Incapacidades { get; set; } = null!;
        // DbSet
        public DbSet<VacacionesEmpleado> Vacaciones { get; set; } = null!;
        public DbSet<Tarea> Tareas { get; set; } = null!;

    }
}
