using Microsoft.EntityFrameworkCore.Migrations;

namespace skyforger.Migrations.Players
{
    public partial class PlayersInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Auth0Id = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: false),
                    CharacterName = table.Column<string>(nullable: false),
                    ProfilePictureUri = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
