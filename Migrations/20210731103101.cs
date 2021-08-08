using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeOffTracker.Migrations
{
    public partial class _2021_07_31 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "_public");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    login = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50,
                        nullable: true),
                    second_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50,
                        nullable: true),
                    password =
                        table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_type_id = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    project_role_comment = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    project_role_type_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    state_detail_id = table.Column<int>(type: "int", nullable: false),
                    date_time_from = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_time_to = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Request_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "Request_User_id_fk",
                        column: x => x.user_id,
                        principalSchema: "_public",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_Signature",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    N_in_queue = table.Column<int>(type: "int", nullable: false),
                    request_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    approved = table.Column<bool>(type: "bit", nullable: false),
                    deleted = table.Column<bool>(type: "bit", nullable: false),
                    reason = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_Signature_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "User_Signature_Request_id_fk",
                        column: x => x.request_id,
                        principalSchema: "_public",
                        principalTable: "Request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "User_Signature_User_id_fk",
                        column: x => x.user_id,
                        principalSchema: "_public",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_project_role_type_id",
                schema: "_public",
                table: "Request",
                column: "project_role_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Request_request_type_id",
                schema: "_public",
                table: "Request",
                column: "request_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Request_state_detail_id",
                schema: "_public",
                table: "Request",
                column: "state_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_Request_user_id",
                schema: "_public",
                table: "Request",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_role_id",
                schema: "_public",
                table: "User",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "User_email_uindex",
                schema: "_public",
                table: "User",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "User_login_uindex",
                schema: "_public",
                table: "User",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Signature_request_id",
                schema: "_public",
                table: "User_Signature",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Signature_user_id",
                schema: "_public",
                table: "User_Signature",
                column: "user_id");

            //Data initialization
            migrationBuilder.Sql(
                @"insert into _public.[User] (email, login, first_name, second_name, password, role_id, deleted) values 
                (N'admin@a.com', N'admin', N'admin', N'admin', N'admin', 1, 0),
                (N'accounting@a.com', N'accounting', N'accounting', N'accounting', N'accounting', 2, 0),
                (N'employee@a.com', N'employee', N'employee', N'employee', N'employee', 3, 0),
                (N'manager@a.com', N'manager', N'manager', N'manager', N'manager', 4, 0);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_Signature",
                schema: "_public");

            migrationBuilder.DropTable(
                name: "Request",
                schema: "_public");

            migrationBuilder.DropTable(
                name: "User",
                schema: "_public");
        }
    }
}