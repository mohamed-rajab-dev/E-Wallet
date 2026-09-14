using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            const long userRoleId = 1;
            const long adminRoleId = 2;


            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { userRoleId, "User", "USER", Guid.NewGuid().ToString() }
            );

            // Insert the Admin role
            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { adminRoleId, "Admin", "ADMIN", Guid.NewGuid().ToString() }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Admin"
            );

            // Delete the Admin role
            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Name",
                keyValue: "User"
            );
        }
    }
}
