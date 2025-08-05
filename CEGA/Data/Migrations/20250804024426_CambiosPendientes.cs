using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CEGA.Data.Migrations
{
    /// <inheritdoc />
    public partial class CambiosPendientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ComentariosPlano",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImagenBase64",
                table: "ComentariosPlano",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ComentariosPlano");

            migrationBuilder.DropColumn(
                name: "ImagenBase64",
                table: "ComentariosPlano");
        }
    }
}
