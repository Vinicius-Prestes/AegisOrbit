using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AegisOrbit.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpacialObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Mass = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Altitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    PositionUpdatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Velocity = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ObjectType = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: false),
                    Operator = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OperatingFrequency = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsSignalActive = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    Origin = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EstimatedSizeMeters = table.Column<double>(type: "BINARY_DOUBLE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpacialObjects", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpacialObjects");
        }
    }
}
