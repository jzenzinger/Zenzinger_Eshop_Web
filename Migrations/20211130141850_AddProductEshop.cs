using Microsoft.EntityFrameworkCore.Migrations;

namespace Zenzinger_Eshop_Web.Migrations
{
    public partial class AddProductEshop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Product",
                newName: "ID");

            migrationBuilder.AddColumn<string>(
                name: "ImageAlt",
                table: "Product",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImageSource",
                table: "Product",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "Product",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Product",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAlt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ImageSource",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Info",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Product",
                newName: "id");
        }
    }
}
