using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace student_crud.Migrations
{
    /// <inheritdoc />
    public partial class teacherChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_ClassSubjects_SubjectId",
                table: "TeacherSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjects_Teacher_TeacherId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_SubjectId",
                table: "TeacherSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjects_TeacherId",
                table: "TeacherSubjects");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TeacherSubjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Teacher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Teacher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Teacher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubjectIds",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "SubjectIds",
                table: "Teacher");


            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TeacherSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SubjectId",
                table: "TeacherSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_TeacherId",
                table: "TeacherSubjects",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_ClassSubjects_SubjectId",
                table: "TeacherSubjects",
                column: "SubjectId",
                principalTable: "ClassSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjects_Teacher_TeacherId",
                table: "TeacherSubjects",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
