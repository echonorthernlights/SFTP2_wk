using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFTP2.Migrations
{
    /// <inheritdoc />
    public partial class mig33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutFlows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ServerAddress = table.Column<string>(type: "TEXT", nullable: false),
                    RemotePath = table.Column<string>(type: "TEXT", nullable: false),
                    InFlowId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutFlows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InFlows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ServerAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ArchivePath = table.Column<string>(type: "TEXT", nullable: false),
                    OutFlowId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InFlows_OutFlows_OutFlowId",
                        column: x => x.OutFlowId,
                        principalTable: "OutFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "OutFlows",
                columns: new[] { "Id", "InFlowId", "RemotePath", "ServerAddress" },
                values: new object[] { "8e4d4ee7-c189-4843-ac39-930eb81b3ec0", "7e34ef79-03ec-4c9a-ba3b-a2458c8c76c3", "/remote/path", "sftp://example.com" });

            migrationBuilder.InsertData(
                table: "InFlows",
                columns: new[] { "Id", "ArchivePath", "OutFlowId", "ServerAddress" },
                values: new object[] { "7e34ef79-03ec-4c9a-ba3b-a2458c8c76c3", "/archive/path", "8e4d4ee7-c189-4843-ac39-930eb81b3ec0", "sftp://example.com" });

            migrationBuilder.CreateIndex(
                name: "IX_InFlows_OutFlowId",
                table: "InFlows",
                column: "OutFlowId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InFlows");

            migrationBuilder.DropTable(
                name: "OutFlows");
        }
    }
}
