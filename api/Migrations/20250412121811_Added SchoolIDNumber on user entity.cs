using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddedSchoolIDNumberonuserentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e6d936a-4f9d-4125-b687-02d46bb44bf1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f08bba20-7781-4bcd-8896-bec9aa8b5091");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fce5a29d-0203-4da0-9337-6913bb8b7fa7");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(44)",
                maxLength: 44,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "SchoolIdNumber",
                table: "AspNetUsers",
                type: "int",
                maxLength: 5,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2cd09e6b-2c7d-48bb-ab2e-dcd4bdf9d1ea", null, "Merchant", "MERCHANT" },
                    { "872df129-d047-435e-8db3-1e75c138ce91", null, "Admin", "ADMIN" },
                    { "d8564fcf-ab5a-4c54-a8f0-ca187f569c99", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2cd09e6b-2c7d-48bb-ab2e-dcd4bdf9d1ea");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "872df129-d047-435e-8db3-1e75c138ce91");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8564fcf-ab5a-4c54-a8f0-ca187f569c99");

            migrationBuilder.DropColumn(
                name: "SchoolIdNumber",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(44)",
                oldMaxLength: 44);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1e6d936a-4f9d-4125-b687-02d46bb44bf1", null, "Merchant", "MERCHANT" },
                    { "f08bba20-7781-4bcd-8896-bec9aa8b5091", null, "Admin", "ADMIN" },
                    { "fce5a29d-0203-4da0-9337-6913bb8b7fa7", null, "User", "USER" }
                });
        }
    }
}
