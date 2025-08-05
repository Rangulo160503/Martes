using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CEGA.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaReporteIncapacidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IncapacidadesEmpleado",
                table: "IncapacidadesEmpleado");

            migrationBuilder.RenameTable(
                name: "IncapacidadesEmpleado",
                newName: "IncapacidadEmpleado");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncapacidadEmpleado",
                table: "IncapacidadEmpleado",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ReportesIncapacidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreReporte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolSeleccionado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesIncapacidades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportesIncapacidades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncapacidadEmpleado",
                table: "IncapacidadEmpleado");

            migrationBuilder.RenameTable(
                name: "IncapacidadEmpleado",
                newName: "IncapacidadesEmpleado");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncapacidadesEmpleado",
                table: "IncapacidadesEmpleado",
                column: "Id");
        }
    }
}
