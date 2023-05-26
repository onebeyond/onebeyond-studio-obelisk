using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneBeyond.Studio.Obelisk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Examples_TodoItems_extra_props_removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "HouseNo",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Zip",
                table: "TodoItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HouseNo",
                table: "TodoItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TodoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
