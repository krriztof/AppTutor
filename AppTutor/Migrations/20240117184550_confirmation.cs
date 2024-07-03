using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppTutor.Migrations
{
    public partial class confirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Teachers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Teachers");
        }
    }
}
