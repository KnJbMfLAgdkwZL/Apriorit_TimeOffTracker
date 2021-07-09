using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeOffTracker.Migrations
{
    public partial class MigrateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "_public");

            migrationBuilder.CreateTable(
                name: "Project_role_type",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    comments = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Project_role_type_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Request_type",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    comments = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Request_type_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "State_detail",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    comments = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("State_detail_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "User_Role",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    comments = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_Role_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "_public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    login = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    second_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "User_User_Role_id_fk",
                        column: x => x.role_id,
                        principalSchema: "_public",
                        principalTable: "User_Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "Request_Project_role_type_id_fk",
                        column: x => x.project_role_type_id,
                        principalSchema: "_public",
                        principalTable: "Project_role_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Request_Request_type_id_fk",
                        column: x => x.request_type_id,
                        principalSchema: "_public",
                        principalTable: "Request_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "Request_State_detail_id_fk",
                        column: x => x.state_detail_id,
                        principalSchema: "_public",
                        principalTable: "State_detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                    deleted = table.Column<bool>(type: "bit", nullable: false)
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
            migrationBuilder.Sql(@"insert into _public.Project_role_type (type, comments, deleted) values 
                (N'No role', N'Нет роли', 0),
                (N'Member', N'Член команды', 0),
                (N'Dedicated', N'Мотивированный, преданный участник команды', 0),
                (N'Representative', N'Представленный заказчику', 0);
            ");
            
            migrationBuilder.Sql(@"insert into _public.Request_type (type, comments, deleted) values 
                (N'Paid holiday', N'Оплачиваемый отпуск', 0),
                (N'Admin (unpaid) planned', N'Административный (неоплачиваемый) плановый отпуск', 0),
                (N'Admin (unpaid) force majeure', N'Административный (неоплачиваемый) отпуск по причине форс-мажора', 0),
                (N'Study', N'Учебный отпуск', 0),
                (N'Social', N'Социальный отпуск (по причине смерти близкого)', 0),
                (N'Sick with docs', N'Больничный с больничным листом', 0),
                (N'Sick without docs', N'Больничный без больничного листа', 0);
            ");
            
            migrationBuilder.Sql(@"insert into _public.State_detail (type, comments, deleted) values 
                (N'New', N'Новая (New) Заявка создана в системе, но не получено еще ни одного утверждения/отказа', 0),
                (N'In progress', N'Заявка получила минимум одно утверждение, но не была утверждена всеми людьми из цепочки менеджеров ', 0),
                (N'Approved', N'Заявка была утверждена всеми людьми из цепочки менеджеров, отпуск считается утвержденным.', 0),
                (N'Rejected', N'Заявка была отклонена кем-то из цепочки менеджеров. Отпуск не утвержден. Сотрудник может составить новую заявку. Заявка также считается отклоненной, если сотрудник лично отменил ее или изменил ее.', 0);
            ");

            migrationBuilder.Sql("insert into _public.User_Role (type, comments, deleted) values" +
                "(N'Admin', N'Администратор (Admin): встроенный пользователь\n" +
                    "Регистрирует пользователя\n" +
                    "определяет роль пользователя: Manager/Employee', 0)," +
                                                              
                "(N'Accounting', N'Бухгалтерия (Accounting): Роль пользователя Бухгалтерия - это обобщенная роль для нескольких людей.\n" +
                    "Первым подписывает заявку на любой отпуск\n" +
                    "Получает нотификацию о полностью подписанной заявке\n" +
                    "Может в любой момент просмотреть статистику заявок', 0)," +
                                                              
                "(N'Employee', N'Сотрудник (Employee):\n" +
                    "Может оставлять заявку на отпуск\n" +
                    "Может просмотреть статистику своих заявок на отпуск', 0)," +
                                                              
                "(N'Manager', N'Менеджер (Manager):\n" +
                    "Подписывает заявку на отпуск\n" +
                    "Может в любой момент просмотреть статистику заявок\n" +
                    "Может оставлять заявки на отпуск, как обычный сотрудник\n" +
                    "Может просмотреть статистику своих заявок на отпуск', 0);"
                );
            
            
            
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
                name: "Project_role_type",
                schema: "_public");

            migrationBuilder.DropTable(
                name: "Request_type",
                schema: "_public");

            migrationBuilder.DropTable(
                name: "State_detail",
                schema: "_public");

            migrationBuilder.DropTable(
                name: "User",
                schema: "_public");

            migrationBuilder.DropTable(
                name: "User_Role",
                schema: "_public");
        }
    }
}
