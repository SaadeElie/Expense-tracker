using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adv._Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserIdNullableInCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true, // Make UserId nullable
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false, // Revert to NOT NULL if rolling back
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

    }
}
