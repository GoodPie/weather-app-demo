using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIso3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Iso3",
                table: "Locations");

            migrationBuilder.AlterColumn<string>(
                name: "Iso2",
                table: "Locations",
                type: "TEXT",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Iso2",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Iso3",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
