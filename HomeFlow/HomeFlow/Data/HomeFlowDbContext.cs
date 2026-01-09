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
namespace HomeFlow.Data;

public class HomeFlowDbContext : DbContext, IHomeFlowDbContext
{
    public HomeFlowDbContext( DbContextOptions options ) : base( options ) { }

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

    public override Task<int> SaveChangesAsync( CancellationToken cancellationToken = default )
    {
        var entries = ChangeTracker.Entries()
            .Where( e => e.Entity is Models.Model && (e.State == EntityState.Added || e.State == EntityState.Modified) );

        foreach ( var entry in entries )
        {
            var entity = (Models.Model) entry.Entity;

            entity.Modified = DateTime.UtcNow;

            if ( entry.State == EntityState.Added )
            {
                entity.Created = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync( cancellationToken );
    }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.UseEntityNameTableConventions();

        modelBuilder.Entity<SystemSettingEntity>( entity =>
        {
            entity.HasIndex( s => s.Key ).IsUnique();
        } );

        modelBuilder.Entity<TagEntity>( entity =>
        {
            entity.HasIndex( t => new { t.Name, t.EntityType, t.EntityId } ).IsUnique();
        } );

        modelBuilder.Entity<ContactEntity>( entity =>
        {
            entity.HasOne( e => e.Image ).WithMany().HasForeignKey( e => e.ImageId );
        } );

        modelBuilder.Entity<RecipeEntity>( entity =>
        {
            entity.HasMany( e => e.RecipeGroceryItems ).WithOne( e => e.Recipe ).HasForeignKey( e => e.RecipeId );
            entity.HasMany( e => e.RecipeSteps ).WithOne( e => e.Recipe ).HasForeignKey( e => e.RecipeId );
            entity.HasOne( e => e.Image ).WithMany().HasForeignKey( e => e.ImageId );
        } );

        modelBuilder.Entity<GroceryStoreEntity>( entity =>
        {
            entity.HasMany( e => e.GroceryStoreAisles ).WithOne( e => e.GroceryStore ).HasForeignKey( e => e.GroceryStoreId );
        } );

        modelBuilder.Entity<GroceryStoreAisleEntity>( entity =>
        {
            entity.HasMany( e => e.GroceryStoreAisleGroceryItems ).WithOne( e => e.GroceryStoreAisle ).HasForeignKey( e => e.GroceryStoreAisleId );
        } );

        modelBuilder.Entity<GroceryStoreAisleGroceryItemEntity>( entity =>
        {
            entity.HasKey( e => new { e.GroceryStoreAisleId, e.GroceryItemId } );
            entity.HasOne( e => e.GroceryStoreAisle ).WithMany( e => e.GroceryStoreAisleGroceryItems ).HasForeignKey( e => e.GroceryStoreAisleId );
            entity.HasOne( e => e.GroceryItem ).WithMany( e => e.GroceryStoreAisleGroceryItems ).HasForeignKey( e => e.GroceryItemId );
        } );

        modelBuilder.Entity<GroceryListEntity>( entity =>
        {
            entity.HasMany( e => e.Items ).WithOne( e => e.GroceryList ).HasForeignKey( e => e.GroceryListId );
        } );

        modelBuilder.Entity<GroceryListItemEntity>( entity =>
        {
            entity.HasOne( e => e.RecipeGroceryItem ).WithMany().HasForeignKey( e => e.RecipeGroceryItemId );
            entity.HasOne( e => e.GroceryItem ).WithMany().HasForeignKey( e => e.GroceryItemId );
            entity.HasOne( e => e.SourceRecipe ).WithMany().HasForeignKey( e => e.SourceRecipeId ).OnDelete( DeleteBehavior.Cascade );
        } );

        modelBuilder.Entity<TodoListEntity>( entity =>
        {
            entity.HasMany( e => e.Items ).WithOne( e => e.TodoList ).HasForeignKey( e => e.TodoListId );
        } );

        modelBuilder.Entity<TodoItemEntity>( entity =>
        {
            entity.HasOne( e => e.AssignedFamilyMember ).WithMany().HasForeignKey( e => e.AssignedFamilyMemberId );
        } );

        base.OnModelCreating( modelBuilder );
    }
}
