using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefreshTokenEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table =>
                    new
                    {
                        Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                        IsUsed = table.Column<bool>(type: "bit", nullable: false),
                        IsInvalidated = table.Column<bool>(type: "bit", nullable: false),
                        Jti = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RefreshTokens");
        }
    }
}
