using Microsoft.EntityFrameworkCore.Migrations;

namespace CarStoreWeb.Migrations
{
    public partial class WithCarDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Cars",
                newName: "CarDescription_FuelType");

            migrationBuilder.AddColumn<string>(
                name: "CarDescription_Color",
                table: "Cars",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CarDescription_EngineСapacity",
                table: "Cars",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CarDescription_YearOfManufacture",
                table: "Cars",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarDescription_Color",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarDescription_EngineСapacity",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarDescription_YearOfManufacture",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "CarDescription_FuelType",
                table: "Cars",
                newName: "Description");
        }
    }
}
