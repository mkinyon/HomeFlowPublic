using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToGroceryListItemSourceRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroceryListItems_Recipes_SourceRecipeId",
                table: "GroceryListItems");

            migrationBuilder.AddForeignKey(
                name: "FK_GroceryListItems_Recipes_SourceRecipeId",
                table: "GroceryListItems",
                column: "SourceRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroceryListItems_Recipes_SourceRecipeId",
                table: "GroceryListItems");

            migrationBuilder.AddForeignKey(
                name: "FK_GroceryListItems_Recipes_SourceRecipeId",
                table: "GroceryListItems",
                column: "SourceRecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");
        }
    }
}
