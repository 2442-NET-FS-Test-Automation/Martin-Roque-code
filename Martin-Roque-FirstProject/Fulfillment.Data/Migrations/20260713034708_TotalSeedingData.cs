using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fulfillment.Data.Migrations
{
    /// <inheritdoc />
    public partial class TotalSeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BuyingLines_BuyingId",
                table: "BuyingLines");

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[,]
                {
                    { 4, "axel.blaze@gmail.com", "Axel Blaze" },
                    { 5, "nene.kusanagu@gmail.com ", "Nene Kusanagi" }
                });

            migrationBuilder.InsertData(
                table: "Videogames",
                columns: new[] { "Id", "ESRB", "Name", "Price", "SpeIden" },
                values: new object[,]
                {
                    { 5, "E+10", "Splatoon Riders", 65.50m, "VGM-005" },
                    { 6, "M+18", "GTA VI", 99.99m, "VGM-006" }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "Id", "CurrentStock", "ProductId" },
                values: new object[,]
                {
                    { 5, 4, 5 },
                    { 6, 11, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyingLines_BuyingId",
                table: "BuyingLines",
                column: "BuyingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BuyingLines_BuyingId",
                table: "BuyingLines");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Videogames",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Videogames",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.CreateIndex(
                name: "IX_BuyingLines_BuyingId",
                table: "BuyingLines",
                column: "BuyingId",
                unique: true);
        }
    }
}
