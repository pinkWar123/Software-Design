using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehaviourOfStatusTransitionToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusTransitions_Statuses_TargetStatusId",
                table: "StatusTransitions");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusTransitions_Statuses_TargetStatusId",
                table: "StatusTransitions",
                column: "TargetStatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusTransitions_Statuses_TargetStatusId",
                table: "StatusTransitions");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusTransitions_Statuses_TargetStatusId",
                table: "StatusTransitions",
                column: "TargetStatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
