using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adv._Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyBudgetToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyBudget",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyBudget",
                table: "AspNetUsers");
        }
    }
}
