using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToStudentNotificaiton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_Students_StudentId",
                table: "StudentNotification");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_Students_StudentId",
                table: "StudentNotification",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_Students_StudentId",
                table: "StudentNotification");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_Students_StudentId",
                table: "StudentNotification",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId");
        }
    }
}
