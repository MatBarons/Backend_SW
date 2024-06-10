using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Techedge.Template.Api.Net.Repositories.Migrations
{
    /*
     * This is the first migration to create the Example table in the database
     * This has been auto generated with the dotnet cli, can also be generated with VS Package Manager Console
     *
     * To generate a migration you first make changes to your models and context, and then call the dotnet CLI to generate it.
     * You can also write them by hand or change auto-generated migrations to add custom DB modifications
     * Remember to always make both Up and Down methods
     */
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Example",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Example", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Example",
                schema: "dbo");
        }
    }
}
