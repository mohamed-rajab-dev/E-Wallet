using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyKeyInTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_WalletId",
                table: "Transaction");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transaction",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "Transaction",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_WalletId_IdempotencyKey",
                table: "Transaction",
                columns: new[] { "WalletId", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_WalletId_IdempotencyKey",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "Transaction");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transaction",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_WalletId",
                table: "Transaction",
                column: "WalletId");
        }
    }
}
