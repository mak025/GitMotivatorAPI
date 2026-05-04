using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GithubMotivator.Migrations
{
    /// <inheritdoc />
    public partial class Statistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommitsTotal = table.Column<int>(type: "int", nullable: false),
                    PullRequestsTotal = table.Column<int>(type: "int", nullable: false),
                    MergesTotal = table.Column<int>(type: "int", nullable: false),
                    ReviewsTotal = table.Column<int>(type: "int", nullable: false),
                    ContributorsTotal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statistics");
        }
    }
}
