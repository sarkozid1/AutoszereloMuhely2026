using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoszereloMuhely.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ugyfelek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nev = table.Column<string>(type: "TEXT", nullable: false),
                    Lakcim = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ugyfelek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Munkak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UgyfelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rendszam = table.Column<string>(type: "TEXT", nullable: false),
                    GyartasiEv = table.Column<int>(type: "INTEGER", nullable: false),
                    Kategoria = table.Column<string>(type: "TEXT", nullable: false),
                    HibaLeiras = table.Column<string>(type: "TEXT", nullable: false),
                    HibaSulyossag = table.Column<int>(type: "INTEGER", nullable: false),
                    Allapot = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Munkak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Munkak_Ugyfelek_UgyfelId",
                        column: x => x.UgyfelId,
                        principalTable: "Ugyfelek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Munkak_UgyfelId",
                table: "Munkak",
                column: "UgyfelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Munkak");

            migrationBuilder.DropTable(
                name: "Ugyfelek");
        }
    }
}
