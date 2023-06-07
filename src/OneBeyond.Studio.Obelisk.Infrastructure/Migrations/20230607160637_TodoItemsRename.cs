using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneBeyond.Studio.Obelisk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TodoItemsRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItemProperties_TodoItems_TodoItemId",
                table: "TodoItemProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_AssignedToUserId",
                table: "TodoItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoItems",
                table: "TodoItems");

            migrationBuilder.RenameTable(
                name: "TodoItems",
                newName: "MyTodoItems");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItems_Identity",
                table: "MyTodoItems",
                newName: "IX_MyTodoItems_Identity");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItems_AssignedToUserId",
                table: "MyTodoItems",
                newName: "IX_MyTodoItems_AssignedToUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyTodoItems",
                table: "MyTodoItems",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.AddForeignKey(
                name: "FK_MyTodoItems_Users_AssignedToUserId",
                table: "MyTodoItems",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItemProperties_MyTodoItems_TodoItemId",
                table: "TodoItemProperties",
                column: "TodoItemId",
                principalTable: "MyTodoItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyTodoItems_Users_AssignedToUserId",
                table: "MyTodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItemProperties_MyTodoItems_TodoItemId",
                table: "TodoItemProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyTodoItems",
                table: "MyTodoItems");

            migrationBuilder.RenameTable(
                name: "MyTodoItems",
                newName: "TodoItems");

            migrationBuilder.RenameIndex(
                name: "IX_MyTodoItems_Identity",
                table: "TodoItems",
                newName: "IX_TodoItems_Identity");

            migrationBuilder.RenameIndex(
                name: "IX_MyTodoItems_AssignedToUserId",
                table: "TodoItems",
                newName: "IX_TodoItems_AssignedToUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItems",
                table: "TodoItems",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItemProperties_TodoItems_TodoItemId",
                table: "TodoItemProperties",
                column: "TodoItemId",
                principalTable: "TodoItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Users_AssignedToUserId",
                table: "TodoItems",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
