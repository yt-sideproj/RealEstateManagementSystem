using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAddressStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Houses");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Houses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DetailAddress",
                table: "Houses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Houses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "DetailAddress",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Houses");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Houses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
