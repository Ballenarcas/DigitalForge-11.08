using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Votify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrlToEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imagen_url",
                table: "evento",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "evento",
                columns: new[] { "id", "descripcion", "fecha_fin", "fecha_inicio", "imagen_url", "nombre" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), "Evento de programación de 48 horas para crear soluciones innovadoras.", new DateTime(2026, 4, 29, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1373), new DateTime(2026, 4, 26, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1123), "images/hackathon-cover.png", "Hackathon Anual 2026" },
                    { new Guid("f6e5d4c3-b2a1-4f5e-9d8c-7b6a5e4d3c2b"), "Presentación de proyectos finales de la asignatura Proyectos de Software.", new DateTime(2026, 5, 7, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1696), new DateTime(2026, 5, 2, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1695), "images/feria-cover.png", "Feria de Proyectos PSW" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "evento",
                keyColumn: "id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"));

            migrationBuilder.DeleteData(
                table: "evento",
                keyColumn: "id",
                keyValue: new Guid("f6e5d4c3-b2a1-4f5e-9d8c-7b6a5e4d3c2b"));

            migrationBuilder.DropColumn(
                name: "imagen_url",
                table: "evento");
        }
    }
}
