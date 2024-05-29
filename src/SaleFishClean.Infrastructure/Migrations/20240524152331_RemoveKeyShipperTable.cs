using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaleFishClean.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveKeyShipperTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Shippers",
                table: "Shippers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Shippers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shippers",
                table: "Shippers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Shippers_ShipperId",
                table: "Shippers",
                column: "ShipperId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Shippers",
                table: "Shippers");

            migrationBuilder.DropIndex(
                name: "IX_Shippers_ShipperId",
                table: "Shippers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Shippers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shippers",
                table: "Shippers",
                column: "ShipperId");
        }
    }
}
