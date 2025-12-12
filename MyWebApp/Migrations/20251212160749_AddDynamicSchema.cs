using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace web_app_slk.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaColumns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaTableId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DataType = table.Column<string>(type: "text", nullable: false),
                    LinkedTableId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaColumns_MetaTables_MetaTableId",
                        column: x => x.MetaTableId,
                        principalTable: "MetaTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaTableId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaRows_MetaTables_MetaTableId",
                        column: x => x.MetaTableId,
                        principalTable: "MetaTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaRowId = table.Column<int>(type: "integer", nullable: false),
                    MetaColumnId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaValues_MetaColumns_MetaColumnId",
                        column: x => x.MetaColumnId,
                        principalTable: "MetaColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MetaValues_MetaRows_MetaRowId",
                        column: x => x.MetaRowId,
                        principalTable: "MetaRows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetaColumns_MetaTableId",
                table: "MetaColumns",
                column: "MetaTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaRows_MetaTableId",
                table: "MetaRows",
                column: "MetaTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaValues_MetaColumnId",
                table: "MetaValues",
                column: "MetaColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaValues_MetaRowId",
                table: "MetaValues",
                column: "MetaRowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaValues");

            migrationBuilder.DropTable(
                name: "MetaColumns");

            migrationBuilder.DropTable(
                name: "MetaRows");

            migrationBuilder.DropTable(
                name: "MetaTables");
        }
    }
}
