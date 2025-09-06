using CEGA.Models;
using CEGA.Models.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        public DbSet<Pdf> Pdfs { get; set; } = null!;
        public DbSet<Anotacion> Anotaciones { get; set; } = null!;
        public DbSet<Proyecto> Proyectos { get; set; } = null!;
        public DbSet<ProyectoEmpleado> ProyectoEmpleados { get; set; } = null!;
        public DbSet<Descarga> Descargas { get; set; } = null!;
        public DbSet<Accidente> Accidentes { get; set; } = null!;
    }
}
