using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneBeyond.Studio.Obelisk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthTokenReplayAttack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Refreshed",
                table: "AuthTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AuthTokens_RefreshToken",
                table: "AuthTokens",
                column: "RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthTokens_RefreshToken",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "Refreshed",
                table: "AuthTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);
        }
    }
}
