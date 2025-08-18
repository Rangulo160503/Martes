using CEGA.Models;
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

        public DbSet<CampMarketing> CampsMarketing { get; set; }
        public DbSet<PoolCorreo> PoolsCorreo { get; set; }
        public DbSet<ProgramacionDistribucion> ProgramacionesDistribucion { get; set; }
        public DbSet<DistribucionMarketing> DistribucionesMarketing { get; set; }
        public DbSet<ClienteMarketing> ClientesMarketing { get; set; }
        public DbSet<EmpleadoSalario> EmpleadosSalarios { get; set; }
        public DbSet<VacacionesEmpleado> VacacionesEmpleados { get; set; }
        public DbSet<PuestoEmpleado> PuestosEmpleado { get; set; }
        public DbSet<HistorialPuestos> HistorialPuestos { get; set; }
        public DbSet<IncapacidadEmpleado> IncapacidadesEmpleado { get; set; }
        public DbSet<Ingreso> Ingresos { get; set; }
        public DbSet<Egreso> Egresos { get; set; }
        public DbSet<CierreDiario> CierresDiarios { get; set; }
        public DbSet<CierreRango> CierresRango { get; set; }
        public DbSet<IncapacidadEmpleado> Incapacidades { get; set; }
        public DbSet<ReporteIncapacidad> ReportesIncapacidades { get; set; }
        public DbSet<ReporteIncidente> ReportesIncidentes { get; set; }
        public DbSet<ReporteEntradaSalida> ReportesEntradasSalidas { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<ComentariosProyecto> ComentariosProyecto { get; set; }
        public DbSet<TareasProyecto> TareasProyecto { get; set; }
        public DbSet<AsignacionesTareaEmpleado> AsignacionesTareaEmpleado { get; set; }
        public DbSet<Plano> Planos { get; set; }
        public DbSet<ComentarioPlano> ComentariosPlano { get; set; }
        public DbSet<TareaPlano> TareasPlano { get; set; }
        public DbSet<TareasProyecto> TareaProyectos { get; set; }    
        public DbSet<ComentariosProyecto> ComentarioProyectos { get; set; }
    }
}
