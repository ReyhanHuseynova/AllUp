using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _allup.Migrations
{
    public partial class AddIsDeactiveColumnToTagTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeactive",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeactive",
                table: "Tags");
        }
    }
}
