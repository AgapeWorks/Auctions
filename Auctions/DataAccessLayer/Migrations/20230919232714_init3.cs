using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodsString",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchasesString",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethodsString",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PurchasesString",
                table: "Users");
        }
    }
}
