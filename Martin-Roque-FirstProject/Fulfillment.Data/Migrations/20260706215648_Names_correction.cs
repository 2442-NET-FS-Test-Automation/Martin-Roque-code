using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fulfillment.Data.Migrations
{
    /// <inheritdoc />
    public partial class Names_correction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Lara Croft");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Lara Crof");
        }
    }
}
