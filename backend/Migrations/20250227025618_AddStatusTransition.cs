using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusTransition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatusTransitions",
                columns: table => new
                {
                    SourceStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetStatusId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTransitions", x => new { x.SourceStatusId, x.TargetStatusId });
                    table.ForeignKey(
                        name: "FK_StatusTransitions_Statuses_TargetStatusId",
                        column: x => x.TargetStatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatusTransitions_TargetStatusId",
                table: "StatusTransitions",
                column: "TargetStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusTransitions");
        }
    }
}
