using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Düzenleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OnayDurumuId",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$CW1J0a/by7LsQMhv1111.e2oWHyCIa2yFec7nuNmbMz/5C/BWjgn6", "$2a$11$CW1J0a/by7LsQMhv1111.e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OnayDurumuId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "UserHash", "UserSalt" },
                values: new object[] { "$2a$11$6U7ROVnCWAjGs9chevycC.zMXlayMyqXOIZk86Q0SbUZcnUsC/JQe", "$2a$11$6U7ROVnCWAjGs9chevycC." });
        }
    }
}
