using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "evento",
                keyColumn: "id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"),
                columns: new[] { "fecha_fin", "fecha_inicio" },
                values: new object[] { new DateTime(2026, 4, 29, 18, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "evento",
                keyColumn: "id",
                keyValue: new Guid("f6e5d4c3-b2a1-4f5e-9d8c-7b6a5e4d3c2b"),
                columns: new[] { "fecha_fin", "fecha_inicio" },
                values: new object[] { new DateTime(2026, 5, 10, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 9, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "evento",
                keyColumn: "id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"),
                columns: new[] { "fecha_fin", "fecha_inicio" },
                values: new object[] { new DateTime(2026, 4, 29, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1373), new DateTime(2026, 4, 26, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1123) });

            migrationBuilder.UpdateData(
                table: "evento",
                keyColumn: "id",
                keyValue: new Guid("f6e5d4c3-b2a1-4f5e-9d8c-7b6a5e4d3c2b"),
                columns: new[] { "fecha_fin", "fecha_inicio" },
                values: new object[] { new DateTime(2026, 5, 7, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1696), new DateTime(2026, 5, 2, 15, 10, 49, 601, DateTimeKind.Utc).AddTicks(1695) });
        }
    }
}
