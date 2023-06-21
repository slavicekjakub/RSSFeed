using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RSSFeed.Migrations
{
    public partial class AddSelected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "RSSFeeds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "RSSFeeds");
        }
    }
}
