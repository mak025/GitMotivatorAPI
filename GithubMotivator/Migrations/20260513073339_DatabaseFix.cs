using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GithubMotivator.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Milestone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommitTreshold = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepositoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Milestone_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_RepositoryId",
                table: "Milestone",
                column: "RepositoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Milestone");
        }
    }
}
