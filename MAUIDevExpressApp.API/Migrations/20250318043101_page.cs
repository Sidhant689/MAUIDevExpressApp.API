using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#nullable disable

namespace MAUIDevExpressApp.API.Migrations
{
    /// <inheritdoc />
    public partial class page : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Page table first
            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Page_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index on ModuleId
            migrationBuilder.CreateIndex(
                name: "IX_Page_ModuleId",
                table: "Page",
                column: "ModuleId");

            // Copy data from Modules to Page
            migrationBuilder.Sql(@"
                INSERT INTO ""Page"" (""Id"", ""Name"", ""Description"", ""ModuleId"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
                SELECT ""Id"", ""Name"", ""Description"", ""Id"", ""IsActive"", ""CreatedAt"", ""UpdatedAt""
                FROM ""Modules""
            ");

            // Now rename ModuleId to PageId in Permissions table
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Modules_ModuleId",
                table: "Permissions");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "Permissions",
                newName: "PageId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_ModuleId",
                table: "Permissions",
                newName: "IX_Permissions_PageId");

            // Add foreign key after data has been copied
            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Page_PageId",
                table: "Permissions",
                column: "PageId",
                principalTable: "Page",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Page_PageId",
                table: "Permissions");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.RenameColumn(
                name: "PageId",
                table: "Permissions",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_PageId",
                table: "Permissions",
                newName: "IX_Permissions_ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Modules_ModuleId",
                table: "Permissions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}