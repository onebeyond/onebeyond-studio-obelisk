using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneBeyond.Studio.Obelisk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TodoPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "MyTodoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MyTodoItems");
        }
    }
}
