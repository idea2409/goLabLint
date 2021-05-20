using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace golablint.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image = table.Column<byte[]>(type: "bytea", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    surname = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Borrowing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipmentid = table.Column<Guid>(type: "uuid", nullable: true),
                    userid = table.Column<Guid>(type: "uuid", nullable: true),
                    startDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    endDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowing", x => x.id);
                    table.ForeignKey(
                        name: "FK_Borrowing_Equipment_equipmentid",
                        column: x => x.equipmentid,
                        principalTable: "Equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Borrowing_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "id", "email", "name", "password", "role", "surname" },
                values: new object[] { new Guid("2315cefe-d411-4bda-aa61-cba2ad15ccc5"), "admin@kmitl.ac.th", "admin", "$2a$11$sXQ/bAumkRok2aLKobk42uOIawhfmUpmt89OrBO7xzO5ULJ7zYMBu", "Admin", "golablint" });

            migrationBuilder.CreateIndex(
                name: "IX_Borrowing_equipmentid",
                table: "Borrowing",
                column: "equipmentid");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowing_userid",
                table: "Borrowing",
                column: "userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Borrowing");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
