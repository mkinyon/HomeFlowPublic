using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFlow.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.CreateTable(
                name: "GroceryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Name = table.Column<string>( type: "TEXT", nullable: false ),
                    Type = table.Column<int>( type: "INTEGER", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_GroceryItems", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "GroceryLists",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Name = table.Column<string>( type: "TEXT", nullable: false ),
                    StartDate = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    EndDate = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    IsPrinted = table.Column<bool>( type: "INTEGER", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_GroceryLists", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "GroceryStores",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Name = table.Column<string>( type: "TEXT", nullable: false ),
                    Location = table.Column<string>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_GroceryStores", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "ImageFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Path = table.Column<string>( type: "TEXT", nullable: false ),
                    MimeType = table.Column<string>( type: "TEXT", nullable: false ),
                    Url = table.Column<string>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ImageFiles", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Key = table.Column<string>( type: "TEXT", nullable: false ),
                    Value = table.Column<string>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_SystemSettings", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Name = table.Column<string>( type: "TEXT", nullable: false ),
                    EntityType = table.Column<string>( type: "TEXT", nullable: false ),
                    EntityId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_Tags", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "GroceryStoreAisles",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Name = table.Column<string>( type: "TEXT", nullable: false ),
                    Order = table.Column<int>( type: "INTEGER", nullable: false ),
                    GroceryStoreId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_GroceryStoreAisles", x => x.Id );
                    table.ForeignKey(
                        name: "FK_GroceryStoreAisles_GroceryStores_GroceryStoreId",
                        column: x => x.GroceryStoreId,
                        principalTable: "GroceryStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "FamilyMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    FirstName = table.Column<string>( type: "TEXT", nullable: false ),
                    LastName = table.Column<string>( type: "TEXT", nullable: false ),
                    ImageId = table.Column<Guid>( type: "TEXT", nullable: true ),
                    Gender = table.Column<int>( type: "INTEGER", nullable: false ),
                    FamilyMemberType = table.Column<int>( type: "INTEGER", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_FamilyMembers", x => x.Id );
                    table.ForeignKey(
                        name: "FK_FamilyMembers_ImageFiles_ImageId",
                        column: x => x.ImageId,
                        principalTable: "ImageFiles",
                        principalColumn: "Id" );
                } );

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Name = table.Column<string>( type: "TEXT", nullable: false ),
                    RecipeType = table.Column<int>( type: "INTEGER", nullable: false ),
                    Description = table.Column<string>( type: "TEXT", nullable: false ),
                    Author = table.Column<string>( type: "TEXT", nullable: false ),
                    Servings = table.Column<int>( type: "INTEGER", nullable: false ),
                    PrepTimeInMinutes = table.Column<int>( type: "INTEGER", nullable: false ),
                    CookTimeInMinutes = table.Column<int>( type: "INTEGER", nullable: false ),
                    TotalTimeInMinutes = table.Column<int>( type: "INTEGER", nullable: false ),
                    ImageId = table.Column<Guid>( type: "TEXT", nullable: true ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_Recipes", x => x.Id );
                    table.ForeignKey(
                        name: "FK_Recipes_ImageFiles_ImageId",
                        column: x => x.ImageId,
                        principalTable: "ImageFiles",
                        principalColumn: "Id" );
                } );

            migrationBuilder.CreateTable(
                name: "GroceryStoreAisleGroceryItems",
                columns: table => new
                {
                    GroceryStoreAisleId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    GroceryItemId = table.Column<Guid>( type: "TEXT", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_GroceryStoreAisleGroceryItems", x => new { x.GroceryStoreAisleId, x.GroceryItemId } );
                    table.ForeignKey(
                        name: "FK_GroceryStoreAisleGroceryItems_GroceryItems_GroceryItemId",
                        column: x => x.GroceryItemId,
                        principalTable: "GroceryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                    table.ForeignKey(
                        name: "FK_GroceryStoreAisleGroceryItems_GroceryStoreAisles_GroceryStoreAisleId",
                        column: x => x.GroceryStoreAisleId,
                        principalTable: "GroceryStoreAisles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "MealPlannerItems",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Date = table.Column<DateOnly>( type: "TEXT", nullable: false ),
                    RecipeId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_MealPlannerItems", x => x.Id );
                    table.ForeignKey(
                        name: "FK_MealPlannerItems_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "RecipeGroceryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Quantity = table.Column<int>( type: "INTEGER", nullable: false ),
                    MeasurementFraction = table.Column<int>( type: "INTEGER", nullable: false ),
                    MeasurementType = table.Column<int>( type: "INTEGER", nullable: false ),
                    AdditionalDetail = table.Column<string>( type: "TEXT", nullable: false ),
                    RecipeId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    GroceryItemId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_RecipeGroceryItems", x => x.Id );
                    table.ForeignKey(
                        name: "FK_RecipeGroceryItems_GroceryItems_GroceryItemId",
                        column: x => x.GroceryItemId,
                        principalTable: "GroceryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                    table.ForeignKey(
                        name: "FK_RecipeGroceryItems_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "RecipeSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    RecipeId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Text = table.Column<string>( type: "TEXT", nullable: false ),
                    Order = table.Column<int>( type: "INTEGER", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_RecipeSteps", x => x.Id );
                    table.ForeignKey(
                        name: "FK_RecipeSteps_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "GroceryListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "TEXT", nullable: false ),
                    GroceryListId = table.Column<Guid>( type: "TEXT", nullable: false ),
                    Text = table.Column<string>( type: "TEXT", nullable: false ),
                    SourceRecipeId = table.Column<Guid>( type: "TEXT", nullable: true ),
                    RecipeGroceryItemId = table.Column<Guid>( type: "TEXT", nullable: true ),
                    GroceryItemId = table.Column<Guid>( type: "TEXT", nullable: true ),
                    Quantity = table.Column<int>( type: "INTEGER", nullable: false ),
                    AdditionalInfo = table.Column<string>( type: "TEXT", nullable: false ),
                    Created = table.Column<DateTime>( type: "TEXT", nullable: false ),
                    Modified = table.Column<DateTime>( type: "TEXT", nullable: true ),
                    ForeignId = table.Column<Guid>( type: "TEXT", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_GroceryListItems", x => x.Id );
                    table.ForeignKey(
                        name: "FK_GroceryListItems_GroceryItems_GroceryItemId",
                        column: x => x.GroceryItemId,
                        principalTable: "GroceryItems",
                        principalColumn: "Id" );
                    table.ForeignKey(
                        name: "FK_GroceryListItems_GroceryLists_GroceryListId",
                        column: x => x.GroceryListId,
                        principalTable: "GroceryLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                    table.ForeignKey(
                        name: "FK_GroceryListItems_RecipeGroceryItems_RecipeGroceryItemId",
                        column: x => x.RecipeGroceryItemId,
                        principalTable: "RecipeGroceryItems",
                        principalColumn: "Id" );
                    table.ForeignKey(
                        name: "FK_GroceryListItems_Recipes_SourceRecipeId",
                        column: x => x.SourceRecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id" );
                } );

            migrationBuilder.CreateIndex(
                name: "IX_FamilyMembers_ImageId",
                table: "FamilyMembers",
                column: "ImageId" );

            migrationBuilder.CreateIndex(
                name: "IX_GroceryListItems_GroceryItemId",
                table: "GroceryListItems",
                column: "GroceryItemId" );

            migrationBuilder.CreateIndex(
                name: "IX_GroceryListItems_GroceryListId",
                table: "GroceryListItems",
                column: "GroceryListId" );

            migrationBuilder.CreateIndex(
                name: "IX_GroceryListItems_RecipeGroceryItemId",
                table: "GroceryListItems",
                column: "RecipeGroceryItemId" );

            migrationBuilder.CreateIndex(
                name: "IX_GroceryListItems_SourceRecipeId",
                table: "GroceryListItems",
                column: "SourceRecipeId" );

            migrationBuilder.CreateIndex(
                name: "IX_GroceryStoreAisleGroceryItems_GroceryItemId",
                table: "GroceryStoreAisleGroceryItems",
                column: "GroceryItemId" );

            migrationBuilder.CreateIndex(
                name: "IX_GroceryStoreAisles_GroceryStoreId",
                table: "GroceryStoreAisles",
                column: "GroceryStoreId" );

            migrationBuilder.CreateIndex(
                name: "IX_MealPlannerItems_RecipeId",
                table: "MealPlannerItems",
                column: "RecipeId" );

            migrationBuilder.CreateIndex(
                name: "IX_RecipeGroceryItems_GroceryItemId",
                table: "RecipeGroceryItems",
                column: "GroceryItemId" );

            migrationBuilder.CreateIndex(
                name: "IX_RecipeGroceryItems_RecipeId",
                table: "RecipeGroceryItems",
                column: "RecipeId" );

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ImageId",
                table: "Recipes",
                column: "ImageId" );

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSteps_RecipeId",
                table: "RecipeSteps",
                column: "RecipeId" );

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name_EntityType_EntityId",
                table: "Tags",
                columns: new[] { "Name", "EntityType", "EntityId" },
                unique: true );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable(
                name: "FamilyMembers" );

            migrationBuilder.DropTable(
                name: "GroceryListItems" );

            migrationBuilder.DropTable(
                name: "GroceryStoreAisleGroceryItems" );

            migrationBuilder.DropTable(
                name: "MealPlannerItems" );

            migrationBuilder.DropTable(
                name: "RecipeSteps" );

            migrationBuilder.DropTable(
                name: "SystemSettings" );

            migrationBuilder.DropTable(
                name: "Tags" );

            migrationBuilder.DropTable(
                name: "GroceryLists" );

            migrationBuilder.DropTable(
                name: "RecipeGroceryItems" );

            migrationBuilder.DropTable(
                name: "GroceryStoreAisles" );

            migrationBuilder.DropTable(
                name: "GroceryItems" );

            migrationBuilder.DropTable(
                name: "Recipes" );

            migrationBuilder.DropTable(
                name: "GroceryStores" );

            migrationBuilder.DropTable(
                name: "ImageFiles" );
        }
    }
}
