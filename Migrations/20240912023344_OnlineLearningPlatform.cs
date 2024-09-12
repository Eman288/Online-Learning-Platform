using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_Learning_Platform.Migrations
{
    public partial class OnlineLearningPlatform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseProviders",
                columns: table => new
                {
                    CourseProviderId = table.Column<string>(type: "varchar(50)", nullable: false),
                    CourseProviderName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CourseProviderImage = table.Column<string>(type: "varchar(250)", nullable: false),
                    CourseProviderDescription = table.Column<string>(type: "varchar(250)", nullable: false),
                    CourseProviderEmail = table.Column<string>(type: "varchar(50)", nullable: false),
                    CourseProviderPassword = table.Column<string>(type: "varchar(150)", nullable: false),
                    CourseProviderBirthday = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProviders", x => x.CourseProviderId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(50)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    UserImage = table.Column<string>(type: "varchar(250)", nullable: true),
                    UserDescription = table.Column<string>(type: "varchar(250)", nullable: false),
                    UserEmail = table.Column<string>(type: "varchar(50)", nullable: false),
                    UserPassword = table.Column<string>(type: "varchar(150)", nullable: false),
                    UserBirthday = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<string>(type: "varchar(50)", nullable: false),
                    CourseName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CourseDescription = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    CourseImage = table.Column<string>(type: "varchar(250)", nullable: false),
                    CourseCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CourseUpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CourseProviderId = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_CourseProviders_CourseProviderId",
                        column: x => x.CourseProviderId,
                        principalTable: "CourseProviders",
                        principalColumn: "CourseProviderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    LessonId = table.Column<string>(type: "varchar(50)", nullable: false),
                    LessonName = table.Column<string>(type: "varchar(50)", nullable: false),
                    LessonDescription = table.Column<string>(type: "varchar(250)", nullable: false),
                    LessonVideo = table.Column<string>(type: "varchar(250)", nullable: false),
                    LessonImage = table.Column<string>(type: "varchar(250)", nullable: false),
                    LessonCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LessonUpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LessonIsDone = table.Column<bool>(type: "bit", nullable: false),
                    CourseId = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.LessonId);
                    table.ForeignKey(
                        name: "FK_Lessons_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCourses",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(50)", nullable: false),
                    CourseId = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourses", x => new { x.UserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_UserCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseProviderId",
                table: "Courses",
                column: "CourseProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourses_CourseId",
                table: "UserCourses",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "UserCourses");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CourseProviders");
        }
    }
}
