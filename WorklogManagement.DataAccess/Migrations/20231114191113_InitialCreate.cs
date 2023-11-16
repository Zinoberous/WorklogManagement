using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorklogManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEntryType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEntryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    CalendarEntryTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEntry", x => x.Id);
                    table.ForeignKey(
                        name: "UX_Day_Date_CalendarEntryType_CalendarEntryTypeId",
                        column: x => x.CalendarEntryTypeId,
                        principalTable: "CalendarEntryType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RefId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    TicketStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_RefId_Ticket_Id",
                        column: x => x.RefId,
                        principalTable: "Ticket",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ticket_TicketStatusId_TicketStatus_Id",
                        column: x => x.TicketStatusId,
                        principalTable: "TicketStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TicketId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAttachment_TicketId_Ticket_Id",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketStatusLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TicketId = table.Column<int>(type: "INTEGER", nullable: false),
                    TicketStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatusLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketStatusLog_TicketId_Ticket_Id",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketStatusLog_TicketStatusId_TicketStatus_Id",
                        column: x => x.TicketStatusId,
                        principalTable: "TicketStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Worklog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    TicketId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    TimeSpent = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worklog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worklog_TicketId_Ticket_Id",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorklogAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorklogId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorklogAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorklogAttachment_WorklogId_Worklog_Id",
                        column: x => x.WorklogId,
                        principalTable: "Worklog",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEntry_CalendarEntryTypeId",
                table: "CalendarEntry",
                column: "CalendarEntryTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_Day_Date_CalendarEntryTypeId",
                table: "CalendarEntry",
                columns: new[] { "Date", "CalendarEntryTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_CalendarEntryType_Name",
                table: "CalendarEntryType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_RefId",
                table: "Ticket",
                column: "RefId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketStatusId",
                table: "Ticket",
                column: "TicketStatusId");

            migrationBuilder.CreateIndex(
                name: "UX_TicketAttachment_TicketId_Name",
                table: "TicketAttachment",
                columns: new[] { "TicketId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TicketStatus_Name",
                table: "TicketStatus",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusLog_TicketId",
                table: "TicketStatusLog",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusLog_TicketStatusId",
                table: "TicketStatusLog",
                column: "TicketStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Worklog_TicketId",
                table: "Worklog",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "UX_WorklogAttachment_WorklogId_Name",
                table: "WorklogAttachment",
                columns: new[] { "WorklogId", "Name" },
                unique: true);

            migrationBuilder.InsertData(
                table: "CalendarEntryType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "WorkTime" },
                    { 2, "Office" },
                    { 3, "Mobile" },
                    { 4, "Holiday" },
                    { 5, "Vacation" },
                    { 6, "TimeCompensation" },
                    { 7, "Ill" },
                });

            migrationBuilder.InsertData(
                table: "TicketStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Todo" },
                    { 2, "Running" },
                    { 3, "Paused" },
                    { 4, "Blocked" },
                    { 5, "Done" },
                    { 6, "Canceled" },
                    { 7, "Continuous" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEntry");

            migrationBuilder.DropTable(
                name: "TicketAttachment");

            migrationBuilder.DropTable(
                name: "TicketStatusLog");

            migrationBuilder.DropTable(
                name: "WorklogAttachment");

            migrationBuilder.DropTable(
                name: "CalendarEntryType");

            migrationBuilder.DropTable(
                name: "Worklog");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "TicketStatus");
        }
    }
}
