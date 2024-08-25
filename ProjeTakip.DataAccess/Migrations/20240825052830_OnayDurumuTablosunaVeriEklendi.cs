using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OnayDurumuTablosunaVeriEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OnayDurumu",
                columns: new[] { "OnayDurumuId", "OnayDurumuAdi" },
                values: new object[] { 4, "Onay durumu yok" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$QiFjzoXGabcyb8YbDorn4e41LnxFwI0Te0ncNaT/tITlO9qWj2soC", "$2a$11$QiFjzoXGabcyb8YbDorn4e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OnayDurumu",
                keyColumn: "OnayDurumuId",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$CW1J0a/by7LsQMhv1111.e2oWHyCIa2yFec7nuNmbMz/5C/BWjgn6", "$2a$11$CW1J0a/by7LsQMhv1111.e" });
        }
    }
}
