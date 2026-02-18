using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolfClubDB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MembershipNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Handicap = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MembershipNumber);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlayerCount = table.Column<int>(type: "int", nullable: false),
                    Player2Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Player2Handicap = table.Column<int>(type: "int", nullable: true),
                    Player3Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Player3Handicap = table.Column<int>(type: "int", nullable: true),
                    Player4Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Player4Handicap = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MembershipNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MemberId_BookingDate",
                table: "Bookings",
                columns: new[] { "MemberId", "BookingDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
