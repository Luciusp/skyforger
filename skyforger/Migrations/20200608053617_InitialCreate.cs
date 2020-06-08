using Microsoft.EntityFrameworkCore.Migrations;

namespace skyforger.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spells",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    ManaDescription = table.Column<string>(nullable: true),
                    SpellLevel = table.Column<int>(nullable: false),
                    SchoolRaw = table.Column<string>(nullable: true),
                    Range = table.Column<string>(nullable: true),
                    Target = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    Effect = table.Column<string>(nullable: true),
                    SavingThrow = table.Column<string>(nullable: true),
                    SpellResistance = table.Column<string>(nullable: true),
                    SpellUri = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Focus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FocusName = table.Column<string>(nullable: true),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Focus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Focus_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManaClasses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ManaClassEnum = table.Column<int>(nullable: false),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManaClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManaClasses_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManaTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ManaTypeEnum = table.Column<int>(nullable: false),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManaTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManaTypes_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialComponent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Component = table.Column<string>(nullable: true),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialComponent_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(nullable: false),
                    TimeFactor = table.Column<int>(nullable: false),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellAction_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellComponent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpellComponentEnum = table.Column<int>(nullable: false),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellComponent_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellDescriptors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpellDescriptorEnum = table.Column<int>(nullable: false),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellDescriptors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellDescriptors_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellSchools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpellSchoolEnum = table.Column<int>(nullable: false),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellSchools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellSchools_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpellSubSchools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpellSubSchoolEnum = table.Column<int>(nullable: false),
                    SpellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellSubSchools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpellSubSchools_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Focus_SpellId",
                table: "Focus",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_ManaClasses_SpellId",
                table: "ManaClasses",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_ManaTypes_SpellId",
                table: "ManaTypes",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialComponent_SpellId",
                table: "MaterialComponent",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellAction_SpellId",
                table: "SpellAction",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellComponent_SpellId",
                table: "SpellComponent",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellDescriptors_SpellId",
                table: "SpellDescriptors",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellSchools_SpellId",
                table: "SpellSchools",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_SpellSubSchools_SpellId",
                table: "SpellSubSchools",
                column: "SpellId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Focus");

            migrationBuilder.DropTable(
                name: "ManaClasses");

            migrationBuilder.DropTable(
                name: "ManaTypes");

            migrationBuilder.DropTable(
                name: "MaterialComponent");

            migrationBuilder.DropTable(
                name: "SpellAction");

            migrationBuilder.DropTable(
                name: "SpellComponent");

            migrationBuilder.DropTable(
                name: "SpellDescriptors");

            migrationBuilder.DropTable(
                name: "SpellSchools");

            migrationBuilder.DropTable(
                name: "SpellSubSchools");

            migrationBuilder.DropTable(
                name: "Spells");
        }
    }
}
