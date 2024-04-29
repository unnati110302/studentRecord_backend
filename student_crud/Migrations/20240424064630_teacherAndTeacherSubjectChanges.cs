using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace student_crud.Migrations
{
    /// <inheritdoc />
    public partial class teacherAndTeacherSubjectChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "TeacherSubjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "TeacherSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "TeacherSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "TeacherSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "TeacherSubjects");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "TeacherSubjects");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TeacherSubjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
    }
}
