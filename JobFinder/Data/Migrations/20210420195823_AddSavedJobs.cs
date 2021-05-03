using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JobFinder.Data.Migrations
{
    public partial class AddSavedJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinSalary = table.Column<int>(type: "int", nullable: true),
                    MaxSalary = table.Column<int>(type: "int", nullable: true),
                    CultureName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Schedule = table.Column<int>(type: "int", nullable: true),
                    EmploymentType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserJobsApplied",
                columns: table => new
                {
                    JobsAppliedToID = table.Column<int>(type: "int", nullable: false),
                    UsersAppliedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobsApplied", x => new { x.JobsAppliedToID, x.UsersAppliedById });
                    table.ForeignKey(
                        name: "FK_UserJobsApplied_AspNetUsers_UsersAppliedById",
                        column: x => x.UsersAppliedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJobsApplied_Jobs_JobsAppliedToID",
                        column: x => x.JobsAppliedToID,
                        principalTable: "Jobs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJobsInterviewing",
                columns: table => new
                {
                    JobsInterviewingWithID = table.Column<int>(type: "int", nullable: false),
                    UsersInterviewingId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobsInterviewing", x => new { x.JobsInterviewingWithID, x.UsersInterviewingId });
                    table.ForeignKey(
                        name: "FK_UserJobsInterviewing_AspNetUsers_UsersInterviewingId",
                        column: x => x.UsersInterviewingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJobsInterviewing_Jobs_JobsInterviewingWithID",
                        column: x => x.JobsInterviewingWithID,
                        principalTable: "Jobs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJobsOffered",
                columns: table => new
                {
                    JobsOfferedID = table.Column<int>(type: "int", nullable: false),
                    UsersOfferedToId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobsOffered", x => new { x.JobsOfferedID, x.UsersOfferedToId });
                    table.ForeignKey(
                        name: "FK_UserJobsOffered_AspNetUsers_UsersOfferedToId",
                        column: x => x.UsersOfferedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJobsOffered_Jobs_JobsOfferedID",
                        column: x => x.JobsOfferedID,
                        principalTable: "Jobs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJobsSaved",
                columns: table => new
                {
                    SavedJobsID = table.Column<int>(type: "int", nullable: false),
                    UsersSavedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobsSaved", x => new { x.SavedJobsID, x.UsersSavedById });
                    table.ForeignKey(
                        name: "FK_UserJobsSaved_AspNetUsers_UsersSavedById",
                        column: x => x.UsersSavedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserJobsSaved_Jobs_SavedJobsID",
                        column: x => x.SavedJobsID,
                        principalTable: "Jobs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserJobsApplied_UsersAppliedById",
                table: "UserJobsApplied",
                column: "UsersAppliedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobsInterviewing_UsersInterviewingId",
                table: "UserJobsInterviewing",
                column: "UsersInterviewingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobsOffered_UsersOfferedToId",
                table: "UserJobsOffered",
                column: "UsersOfferedToId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobsSaved_UsersSavedById",
                table: "UserJobsSaved",
                column: "UsersSavedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserJobsApplied");

            migrationBuilder.DropTable(
                name: "UserJobsInterviewing");

            migrationBuilder.DropTable(
                name: "UserJobsOffered");

            migrationBuilder.DropTable(
                name: "UserJobsSaved");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
