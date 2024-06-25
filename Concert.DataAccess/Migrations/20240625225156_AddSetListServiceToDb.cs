using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concert.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSetListServiceToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SetListServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetListServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetListServices_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetListServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetListServices_ApplicationUserId",
                table: "SetListServices",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SetListServices_ServiceId",
                table: "SetListServices",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SetListServices");
        }
    }
}
