using Microsoft.EntityFrameworkCore.Migrations;

namespace skyforger.Migrations.Backpacks
{
    public partial class BackpacksInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Backpacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemName = table.Column<string>(nullable: true),
                    NotesEffect = table.Column<string>(nullable: true),
                    GpValue = table.Column<float>(nullable: false),
                    Quantity = table.Column<float>(nullable: false),
                    Weight = table.Column<float>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Backpacks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Backpacks");
        }
    }
}
