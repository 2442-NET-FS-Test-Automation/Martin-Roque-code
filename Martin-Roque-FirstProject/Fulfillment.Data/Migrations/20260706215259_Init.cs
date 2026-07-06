using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fulfillment.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FulfillmentEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyingId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FulfilledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillmentEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Videogames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpeIden = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ESRB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videogames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buyings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Stauts = table.Column<int>(type: "int", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buyings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buyings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CurrentStock = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Videogames_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Videogames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyingLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyingId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyingLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyingLines_Buyings_BuyingId",
                        column: x => x.BuyingId,
                        principalTable: "Buyings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "lyon.kennedy@gmail.com", "Lyon Kennedy" },
                    { 2, "lara.croft@gmail.com", "Lara Crof" },
                    { 3, "fox.mccloud@gmail.com", "Fox McCloud" }
                });

            migrationBuilder.InsertData(
                table: "Videogames",
                columns: new[] { "Id", "ESRB", "Name", "Price", "SpeIden" },
                values: new object[,]
                {
                    { 1, "E for Everyone", "Donkey Kong Bananza", 78.50m, "VGM-001" },
                    { 2, "+13 years old", "Halo Remaster Collection", 48.90m, "VGM-002" },
                    { 3, "E for Everyone", "Minecraft", 22.75m, "VGM-003" },
                    { 4, "E+10", "Terraria", 15.50m, "VGM-004" }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "Id", "CurrentStock", "ProductId" },
                values: new object[,]
                {
                    { 1, 5, 1 },
                    { 2, 3, 2 },
                    { 3, 9, 3 },
                    { 4, 7, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyingLines_BuyingId",
                table: "BuyingLines",
                column: "BuyingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buyings_CustomerId",
                table: "Buyings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductId",
                table: "Inventory",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videogames_SpeIden",
                table: "Videogames",
                column: "SpeIden",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyingLines");

            migrationBuilder.DropTable(
                name: "FulfillmentEvents");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Buyings");

            migrationBuilder.DropTable(
                name: "Videogames");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
