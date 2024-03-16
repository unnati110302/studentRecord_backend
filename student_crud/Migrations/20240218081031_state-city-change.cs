using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace student_crud.Migrations
{
    /// <inheritdoc />
    public partial class statecitychange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "States",
                newName: "SId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Cities",
                newName: "CId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_States_StateId",
                table: "Cities",
                column: "StateId",
                principalTable: "States",
                principalColumn: "SId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_States_StateId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_StateId",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "SId",
                table: "States",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "CId",
                table: "Cities",
                newName: "ID");
        }
    }
}
