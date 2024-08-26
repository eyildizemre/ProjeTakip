using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OnayDurumu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OnayDurumuId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$FDLHdfQUiwD6hrnKVMk75eK/7K5cVfDjCxJ13K6/IMfNrSdYOWIEK", "$2a$11$FDLHdfQUiwD6hrnKVMk75e" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OnayDurumuId",
                table: "Projects",
                column: "OnayDurumuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_OnayDurumu_OnayDurumuId",
                table: "Projects",
                column: "OnayDurumuId",
                principalTable: "OnayDurumu",
                principalColumn: "OnayDurumuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_OnayDurumu_OnayDurumuId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OnayDurumuId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "OnayDurumuId",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$gkTBE0ZFDNwT4a3rZc2iKO4kiFsCjK12agfZaZGmzKz5GdbFlF24.", "$2a$11$gkTBE0ZFDNwT4a3rZc2iKO" });
        }
    }
}
