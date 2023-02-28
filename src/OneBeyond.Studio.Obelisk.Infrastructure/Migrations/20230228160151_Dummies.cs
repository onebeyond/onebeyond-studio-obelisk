using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneBeyond.Studio.Obelisk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Dummies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dummies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StringValue = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NumericValue = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    BoolValue = table.Column<bool>(type: "bit", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FKValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Identity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dummies", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Dummies_Users_FKValueId",
                        column: x => x.FKValueId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dummies_FKValueId",
                table: "Dummies",
                column: "FKValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Dummies_Identity",
                table: "Dummies",
                column: "Identity")
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dummies");
        }
    }
}
