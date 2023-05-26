using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneBeyond.Studio.Obelisk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Examples_TodoItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNo = table.Column<int>(type: "int", nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Identity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_TodoItems_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TodoItemProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TodoItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Identity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItemProperties", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_TodoItemProperties_TodoItems_TodoItemId",
                        column: x => x.TodoItemId,
                        principalTable: "TodoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItemProperties_Identity",
                table: "TodoItemProperties",
                column: "Identity")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItemProperties_TodoItemId",
                table: "TodoItemProperties",
                column: "TodoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_AssignedToUserId",
                table: "TodoItems",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_Identity",
                table: "TodoItems",
                column: "Identity")
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItemProperties");

            migrationBuilder.DropTable(
                name: "TodoItems");
        }
    }
}
