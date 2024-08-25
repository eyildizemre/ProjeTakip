using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectWithUserTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "UserTeams",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$YqTcQDleUbSbxZqdkYpp9uSdCc.VyQNoYxNGnCRvfyY/jF0k6SEmC", "$2a$11$YqTcQDleUbSbxZqdkYpp9u" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTeams_ProjectId",
                table: "UserTeams",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeams_Projects_ProjectId",
                table: "UserTeams",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTeams_Projects_ProjectId",
                table: "UserTeams");

            migrationBuilder.DropIndex(
                name: "IX_UserTeams_ProjectId",
                table: "UserTeams");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "UserTeams");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$o8fR6PU2y5Q6CZPdkikIAesCVqqJrw8/ma7X33IWUCPXauydVF0WS", "$2a$11$o8fR6PU2y5Q6CZPdkikIAe" });
        }
    }
}
