using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyTree.Infrastructure.Migrations
{
    public partial class CommonEventsToPersons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommonEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonEventToPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommonEventId = table.Column<Guid>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonEventToPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonEventToPersons_CommonEvent_CommonEventId",
                        column: x => x.CommonEventId,
                        principalTable: "CommonEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonEventToPersons_Person_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonEventToPersons_CommonEventId",
                table: "CommonEventToPersons",
                column: "CommonEventId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonEventToPersons_ParticipantId",
                table: "CommonEventToPersons",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonEventToPersons");

            migrationBuilder.DropTable(
                name: "CommonEvent");
        }
    }
}
