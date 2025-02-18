using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAUIDevExpressApp.API.Migrations
{
    /// <inheritdoc />
    public partial class description_ProductCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductCategories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductCategories");
        }
    }
}
