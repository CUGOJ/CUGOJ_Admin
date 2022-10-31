using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CUGOJ_Admin.Data.Migrations
{
    public partial class AddEnvInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Envs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEnvs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnvId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEnvs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEnvs_EnvId",
                table: "UserEnvs",
                column: "EnvId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEnvs_UserId",
                table: "UserEnvs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Envs");

            migrationBuilder.DropTable(
                name: "UserEnvs");
        }
    }
}
