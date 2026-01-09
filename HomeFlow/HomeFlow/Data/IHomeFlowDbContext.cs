
using HomeFlow.Features.Core.FamilyMembers;
using HomeFlow.Features.Core.ImageFiles;
using HomeFlow.Features.Core.SystemSettings;
using HomeFlow.Features.Core.Tags;
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Features.MealPlanning.GroceryLists;
using HomeFlow.Features.MealPlanning.GroceryStores;
using HomeFlow.Features.MealPlanning.MealPlannerItems;
using HomeFlow.Features.MealPlanning.Recipes;
using HomeFlow.Features.People.Contacts;
using HomeFlow.Features.Tasks.TodoLists;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeFlow.Data
{
    public interface IHomeFlowDbContext
    {
        public ChangeTracker ChangeTracker { get; }

        public DbSet<ImageFileEntity> ImageFiles { get; set; }
        public DbSet<FamilyMemberEntity> FamilyMembers { get; set; }
        public DbSet<SystemSettingEntity> SystemSettings { get; set; }
        public DbSet<TagEntity> Tags { get; set; }

        public DbSet<ContactEntity> Contacts { get; set; }

        public DbSet<GroceryStoreEntity> GroceryStores { get; set; }
        public DbSet<GroceryStoreAisleEntity> GroceryStoreAisles { get; set; }
        public DbSet<GroceryStoreAisleGroceryItemEntity> GroceryStoreAisleGroceryItems { get; set; }
        public DbSet<GroceryListEntity> GroceryLists { get; set; }
        public DbSet<GroceryListItemEntity> GroceryListItems { get; set; }
        public DbSet<GroceryItemEntity> GroceryItems { get; set; }
        public DbSet<RecipeEntity> Recipes { get; set; }
        public DbSet<RecipeStepEntity> RecipeSteps { get; set; }
        public DbSet<RecipeGroceryItemEntity> RecipeGroceryItems { get; set; }
        public DbSet<MealPlannerItemEntity> MealPlannerItems { get; set; }

        public DbSet<TodoListEntity> TodoLists { get; set; }
        public DbSet<TodoItemEntity> TodoItems { get; set; }

        Task<int> SaveChangesAsync( CancellationToken cancellationToken = default );
    }
}
