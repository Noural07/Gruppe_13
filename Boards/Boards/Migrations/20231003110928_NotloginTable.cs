using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boards.Migrations
{
    public partial class NotloginTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Board",
                table: "Board");

            migrationBuilder.RenameTable(
                name: "Board",
                newName: "NotLogin");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotLogin",
                table: "NotLogin",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NotLogin",
                table: "NotLogin");

            migrationBuilder.RenameTable(
                name: "NotLogin",
                newName: "Board");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Board",
                table: "Board",
                column: "ID");
        }
    }
}
