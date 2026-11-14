using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWT.Migrations
{
    public partial class AddUniqueUserIdentifiersAndFieldLimits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username", table: "Users", type: "nvarchar(50)",
                maxLength: 50, nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email", table: "Users", type: "nvarchar(254)",
                maxLength: 254, nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Password", table: "Users", type: "nvarchar(100)",
                maxLength: 100, nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Role", table: "Users", type: "nvarchar(20)",
                maxLength: 20, nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email", table: "Users", column: "Email", unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username", table: "Users", column: "Username", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Users_Email", table: "Users");
            migrationBuilder.DropIndex(name: "IX_Users_Username", table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Username", table: "Users", type: "nvarchar(max)",
                nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(50)", oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email", table: "Users", type: "nvarchar(max)",
                nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(254)", oldMaxLength: 254);

            migrationBuilder.AlterColumn<string>(
                name: "Password", table: "Users", type: "nvarchar(max)",
                nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(100)", oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Role", table: "Users", type: "nvarchar(max)",
                nullable: false, oldClrType: typeof(string),
                oldType: "nvarchar(20)", oldMaxLength: 20);
        }
    }
}
