using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddFolderToImageFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the Folder column
            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "ImageFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            // Update existing records to set appropriate folder based on usage
            migrationBuilder.Sql(@"
                UPDATE ImageFiles 
                SET Folder = CASE 
                    WHEN Id IN (SELECT ImageId FROM FamilyMembers WHERE ImageId IS NOT NULL) THEN 'FamilyMembers'
                    WHEN Id IN (SELECT ImageId FROM Recipes WHERE ImageId IS NOT NULL) THEN 'Recipes'
                    WHEN Id IN (SELECT ImageId FROM Contacts WHERE ImageId IS NOT NULL) THEN 'Contacts'
                    ELSE 'General'
                END
                WHERE Folder = ''
            ");

            // Update Path and Url columns to include folder structure
            migrationBuilder.Sql(@"
                UPDATE ImageFiles 
                SET Path = CASE 
                    WHEN Folder = 'FamilyMembers' THEN 'LocalStorage/ImageFiles/FamilyMembers/' || SUBSTR(Path, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'Recipes' THEN 'LocalStorage/ImageFiles/Recipes/' || SUBSTR(Path, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'Contacts' THEN 'LocalStorage/ImageFiles/Contacts/' || SUBSTR(Path, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'GroceryItems' THEN 'LocalStorage/ImageFiles/GroceryItems/' || SUBSTR(Path, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'Whiteboard' THEN 'LocalStorage/ImageFiles/Whiteboard/' || SUBSTR(Path, LENGTH('LocalStorage/ImageFiles/') + 1)
                    ELSE 'LocalStorage/ImageFiles/General/' || SUBSTR(Path, LENGTH('LocalStorage/ImageFiles/') + 1)
                END
                WHERE Path LIKE 'LocalStorage/ImageFiles/%'
            ");

            migrationBuilder.Sql(@"
                UPDATE ImageFiles 
                SET Url = CASE 
                    WHEN Folder = 'FamilyMembers' THEN 'LocalStorage/ImageFiles/FamilyMembers/' || SUBSTR(Url, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'Recipes' THEN 'LocalStorage/ImageFiles/Recipes/' || SUBSTR(Url, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'Contacts' THEN 'LocalStorage/ImageFiles/Contacts/' || SUBSTR(Url, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'GroceryItems' THEN 'LocalStorage/ImageFiles/GroceryItems/' || SUBSTR(Url, LENGTH('LocalStorage/ImageFiles/') + 1)
                    WHEN Folder = 'Whiteboard' THEN 'LocalStorage/ImageFiles/Whiteboard/' || SUBSTR(Url, LENGTH('LocalStorage/ImageFiles/') + 1)
                    ELSE 'LocalStorage/ImageFiles/General/' || SUBSTR(Url, LENGTH('LocalStorage/ImageFiles/') + 1)
                END
                WHERE Url LIKE 'LocalStorage/ImageFiles/%'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Path and Url columns to original structure
            migrationBuilder.Sql(@"
                UPDATE ImageFiles 
                SET Path = 'LocalStorage/ImageFiles/' || SUBSTR(Path, LENGTH('LocalStorage/ImageFiles/') + 1)
                WHERE Path LIKE 'LocalStorage/ImageFiles/%/%'
            ");

            migrationBuilder.Sql(@"
                UPDATE ImageFiles 
                SET Url = 'LocalStorage/ImageFiles/' || SUBSTR(Url, LENGTH('LocalStorage/ImageFiles/') + 1)
                WHERE Url LIKE 'LocalStorage/ImageFiles/%/%'
            ");

            // Drop the Folder column
            migrationBuilder.DropColumn(
                name: "Folder",
                table: "ImageFiles");
        }
    }
}
