using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFlow.Migrations
{
    /// <inheritdoc />
    public partial class RecipeGroceryItemOrderable : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "RecipeGroceryItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0 );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "RecipeGroceryItems" );
        }
    }
}
