using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class WeatherModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApiResponse = table.Column<string>(type: "TEXT", nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Temperature = table.Column<double>(type: "REAL", nullable: true),
                    TemperatureUnit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FeelsLikeTemperature = table.Column<double>(type: "REAL", nullable: true),
                    Humidity = table.Column<double>(type: "REAL", nullable: true),
                    Condition = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConditionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IconUri = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UvIndex = table.Column<double>(type: "REAL", nullable: true),
                    WindSpeed = table.Column<double>(type: "REAL", nullable: true),
                    WindSpeedUnit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    WindDirection = table.Column<double>(type: "REAL", nullable: true),
                    IsDaytime = table.Column<bool>(type: "INTEGER", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherData_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherData_FetchedAt",
                table: "WeatherData",
                column: "FetchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherData_LocationId",
                table: "WeatherData",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherData");
        }
    }
}
