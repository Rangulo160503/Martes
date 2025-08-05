using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CEGA.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaAsignacionTareaEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AsignacionesTareaEmpleado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TareaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesTareaEmpleado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesTareaEmpleado_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignacionesTareaEmpleado_TareasProyecto_TareaId",
                        column: x => x.TareaId,
                        principalTable: "TareasProyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesTareaEmpleado_TareaId",
                table: "AsignacionesTareaEmpleado",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesTareaEmpleado_UsuarioId",
                table: "AsignacionesTareaEmpleado",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesTareaEmpleado");
        }
    }
}
