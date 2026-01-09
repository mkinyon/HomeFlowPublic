using System.IO.Compression;
using System.Text.Json;
using Bogus;
using HomeFlow.Data;
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
using Microsoft.Data.Sqlite;

namespace HomeFlow.DataManagement;

public static class DatabaseHelper
{
    private static readonly string _baseStoragePath = "LocalStorage";

    private static readonly string _dbName = "homeflow.db";
    private static readonly string _baseBackupDir = Path.Combine( _baseStoragePath, "Backups" );
    private static readonly string _dbBaseDir = Path.Combine( _baseStoragePath, "Database" );
    private static readonly string _dbBackupBaseDir = Path.Combine( _dbBaseDir, "DatabaseBackups" );
    private static readonly string _imageBaseDir = Path.Combine( _baseStoragePath, "ImageFiles" );

    public static string GetBackupDirectory()
    {
        return _baseBackupDir;
    }

    public static bool IsDatabaseEmpty( IHomeFlowDbContext context )
    {
        // checking for systems settings is a good indicator of an empty database.. I didn't say great :|
        return !context.SystemSettings.Any();
    }

    public static async Task SeedDatabaseAsync( IHomeFlowDbContext context, IMapper mapper )
    {
        // generate fake family
        var familyMemberFaker = new Faker<FamilyMemberEntity>()
            .RuleFor( f => f.FirstName, f => f.Person.FirstName )
            .RuleFor( f => f.LastName, f => f.Person.LastName )
            .RuleFor( f => f.Image, ( f, r ) =>
            {
                return new ImageFileEntity
                {
                    Url = f.Internet.Avatar()
                };
            } );

        var familyMembers = familyMemberFaker.Generate( 6 );
        foreach ( var familyMember in familyMembers )
        {
            context.FamilyMembers.Add( familyMember );
        }

        // generate contacts
        var contactFaker = new Faker<ContactEntity>()
            .RuleFor( c => c.FirstName, f => f.Person.FirstName )
            .RuleFor( c => c.LastName, f => f.Person.LastName )
            .RuleFor( c => c.Email, ( f, c ) => f.Internet.Email( c.FirstName, c.LastName ) )
            .RuleFor( c => c.PhoneNumber, f => f.Phone.PhoneNumber() )
            .RuleFor( c => c.BirthDate, f => DateOnly.FromDateTime( f.Date.Between( new DateTime( 1940, 1, 1 ), new DateTime( 2010, 1, 1 ) ) ) )
            .RuleFor( c => c.AnniversaryDate, f => DateOnly.FromDateTime( f.Date.Between( new DateTime( 1960, 1, 1 ), new DateTime( 2020, 1, 1 ) ) ) )
            .RuleFor( c => c.Image, ( f, c ) =>
            {
                return new ImageFileEntity
                {
                    Url = f.Internet.Avatar()
                };
            } );

        var contacts = contactFaker.Generate( 10000 );
        foreach ( var contact in contacts )
        {
            context.Contacts.Add( contact );
        }

        // generate grocery items  
        var groceryItemFaker = new Faker<GroceryItemEntity>()
            .RuleFor( i => i.Name, f => f.Commerce.ProductName() );

        var groceryItems = groceryItemFaker.Generate( 100 );
        foreach ( var groceryItem in groceryItems )
        {
            context.GroceryItems.Add( groceryItem );
        }

        // generate recipes  
        var recipeFaker = new Faker<RecipeEntity>()
            .RuleFor( r => r.Name, f => f.Commerce.ProductName() )
            .RuleFor( r => r.Description, f => f.Lorem.Paragraph() )
            .RuleFor( r => r.Author, f => f.Person.FullName )
            .RuleFor( r => r.RecipeType, ( f, r ) => (RecipeType) f.Random.Number( 1, 3 ) )
            .RuleFor( r => r.Servings, f => f.Random.Number( 1, 10 ) )
            .RuleFor( r => r.PrepTimeInMinutes, f => f.Random.Number( 5, 60 ) )
            .RuleFor( r => r.CookTimeInMinutes, f => f.Random.Number( 5, 60 ) )
            .RuleFor( r => r.TotalTimeInMinutes, ( f, r ) => r.PrepTimeInMinutes + r.CookTimeInMinutes )
            .RuleFor( r => r.Image, ( f, r ) =>
            {
                return new ImageFileEntity
                {
                    Url = f.Image.PicsumUrl( 300, 300 )
                };
            } )
            .RuleFor( r => r.RecipeSteps, ( f, r ) =>
            {
                int stepCount = f.Random.Int( 3, 8 );

                return Enumerable
                .Range( 1, stepCount )
                .Select( _ => new RecipeStepEntity
                {
                    Text = f.Lorem.Sentence()
                } )
                .ToList();
            } )
            .RuleFor( r => r.RecipeGroceryItems, ( f, r ) =>
            {
                IList<GroceryItemEntity> randomGroceryItems = f.Random.ListItems( groceryItems, f.Random.Number( 3, 5 ) );

                // create a RecipeGroceryItem for each Grocery Item
                return randomGroceryItems.Select( gItem => new RecipeGroceryItemEntity
                {
                    GroceryItem = gItem,
                    AdditionalDetail = f.Commerce.ProductAdjective(),
                    Quantity = f.Random.Number( 1, 5 ),
                    MeasurementType = MeasurementType.Cups,
                    MeasurementFraction = MeasurementFraction.None
                } )
                .ToList();
            } );

        var recipes = recipeFaker.Generate( 50 );
        foreach ( var recipe in recipes )
        {
            context.Recipes.Add( recipe );
        }

        // generate meal planner items  
        var mealPlannerItemFaker = new Faker<MealPlannerItemEntity>()
            .RuleFor( m => m.Date, f => DateOnly.FromDateTime( f.Date.Between( new DateTime( 2020, 1, 1 ), new DateTime( 2026, 1, 1 ) ) ) )
            .RuleFor( m => m.Recipe, f => recipeFaker.Generate() );

        var mealPlannerItems = mealPlannerItemFaker.Generate( 2000 );
        foreach ( var mealPlannerItem in mealPlannerItems )
        {
            context.MealPlannerItems.Add( mealPlannerItem );
        }

        // generate grocery lists
        var groceryListFaker = new Faker<GroceryListEntity>()
            .RuleFor( g => g.Name, f => $"Grocery List ({f.Date.Between( DateTime.Now.AddDays( -60 ).Date, DateTime.Now.Date )})" )
            .RuleFor( g => g.IsPrinted, f => f.Random.Bool() )
            .RuleFor( g => g.Items, ( f, r ) =>
            {
                var items = new List<GroceryListItemEntity>();
                for ( int i = 0; i < f.Random.Number( 5, 20 ); i++ )
                {
                    var recipe = f.PickRandom( recipes );

                    for ( int ri = 0; ri < f.Random.Number( 3, 8 ); ri++ )
                    {
                        var recipeGroceryItem = f.PickRandom( recipe.RecipeGroceryItems );

                        if ( recipeGroceryItem != null )
                        {
                            var recipeGroceryItemContract = mapper.Map<RecipeGroceryItem>( recipeGroceryItem );

                            items.Add( new GroceryListItemEntity
                            {
                                Text = recipeGroceryItemContract.ToString(),
                                RecipeGroceryItem = recipeGroceryItem,
                                SourceRecipe = recipe,
                            } );
                        }
                    }
                }

                return items;
            } );


        var groceryLists = groceryListFaker.Generate( 100 );
        foreach ( var groceryList in groceryLists )
        {
            context.GroceryLists.Add( groceryList );
        }

        await context.SaveChangesAsync();

        // generate recipe tags
        var recipeIds = context.Recipes.Select( r => r.Id ).ToList();

        var rTagFaker = new Faker<TagEntity>()
            .RuleFor( t => t.Name, f => f.Commerce.ProductAdjective() + f.UniqueIndex.ToString() )
            .RuleFor( t => t.EntityType, "Recipe" )
            .RuleFor( t => t.EntityId, f => f.PickRandom( recipeIds ) );

        var tags = rTagFaker.Generate( 1000 );
        foreach ( var tag in tags )
        {
            context.Tags.Add( tag );
        }

        // generate groceryItem tags
        var groceryItemIds = context.GroceryItems.Select( r => r.Id ).ToList();

        var iTagFaker = new Faker<TagEntity>()
            .RuleFor( t => t.Name, f => f.Commerce.ProductAdjective() + f.UniqueIndex.ToString() )
            .RuleFor( t => t.EntityType, "GroceryItem" )
            .RuleFor( t => t.EntityId, f => f.PickRandom( groceryItemIds ) );

        tags = iTagFaker.Generate( 1000 );
        foreach ( var tag in tags )
        {
            context.Tags.Add( tag );
        }

        // generate sample grocery stores
        var groceryStoreFaker = new Faker<Features.MealPlanning.GroceryStores.GroceryStoreEntity>()
            .RuleFor( g => g.Name, f => f.Company.CompanyName() )
            .RuleFor( g => g.Location, f => f.Address.StreetAddress() )
            .RuleFor( g => g.GroceryStoreAisles, ( f, r ) =>
            {
                var listOfAisles = new List<GroceryStoreAisleEntity>();
                for ( int i = 0; i < 20; i++ )
                {
                    IList<GroceryItemEntity> randomGroceryItems = f.Random.ListItems( groceryItems, f.Random.Number( 5, 15 ) );

                    listOfAisles.Add( new GroceryStoreAisleEntity
                    {
                        Name = $"Aisle {i + 1} ({f.Commerce.Department()})",
                        Order = i,
                        GroceryStoreAisleGroceryItems = randomGroceryItems
                           .Select( groceryItem => new GroceryStoreAisleGroceryItemEntity
                           {
                               GroceryStoreAisle = new GroceryStoreAisleEntity(),
                               GroceryItem = groceryItem
                           } )
                           .ToList()
                    } );
                }

                return listOfAisles;
            } );

        var stores = groceryStoreFaker.Generate( 10 );
        foreach ( var store in stores )
        {
            context.GroceryStores.Add( store );
        }

        await context.SaveChangesAsync();

        context.SystemSettings.Add( new SystemSettingEntity
        {
            Key = HomeFlow.Constants.SystemSettings.DATABASE_SEEDED,
            Value = "True"
        } );
        await context.SaveChangesAsync();
    }

    public static async Task ImportDatabaseAsync( Stream fileStream, IHomeFlowDbContext context, ISender sender, IMapper mapper, CancellationToken cancellationToken )
    {
        // extract import data from the zip file
        var importData = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

        // Ensure we can seek—if fileStream isn't seekable, wrap it
        Stream zipSource = fileStream.CanSeek
            ? fileStream
            : new BufferedStream( fileStream );

        using var archive = new ZipArchive( zipSource, ZipArchiveMode.Read, leaveOpen: true );

        foreach ( var entry in archive.Entries )
        {
            if ( !entry.FullName.EndsWith( ".json", StringComparison.OrdinalIgnoreCase ) )
                continue;

            var key = Path.GetFileNameWithoutExtension( entry.FullName );

            // If you really want folders: use Path.GetDirectoryName(entry.FullName) + "/" + key

            if ( !importData.ContainsKey( key ) )
            {
                using var entryStream = entry.Open();
                using var reader = new StreamReader( entryStream );
                var jsonContent = await reader.ReadToEndAsync();

                importData[key] = jsonContent;
            }
        }

        await ImportEntitiesAsync<SystemSetting, SystemSettingEntity>( importData, "systemsettings", context.SystemSettings, async ( setting ) =>
        {
            await sender.Send( new UpdateSystemSettingCommand( setting ) );
        }, cancellationToken );

        await ImportEntitiesAsync<Tag, TagEntity>( importData, "tags", context.Tags, async ( tag ) =>
        {
            await sender.Send( new UpdateTagsCommand( tag.EntityType, tag.EntityId, new List<string> { tag.Name } ) );
        }, cancellationToken );

        await ImportEntitiesAsync<FamilyMember, FamilyMemberEntity>( importData, "familymembers", context.FamilyMembers, async ( familyMember ) =>
        {
            await sender.Send( new CreateFamilyMemberCommand( familyMember ) );
        }, cancellationToken );

        await ImportEntitiesAsync<GroceryItem, GroceryItemEntity>( importData, "groceryitems", context.GroceryItems, async ( groceryItem ) =>
        {
            await sender.Send( new CreateGroceryItemCommand( groceryItem ) );
        }, cancellationToken );

        await ImportEntitiesAsync<Recipe, RecipeEntity>( importData, "recipes", context.Recipes, async ( recipe ) =>
        {
            await sender.Send( new CreateRecipeCommand( recipe ) );
        }, cancellationToken );
    }

    public static async Task<MemoryStream> ExportDatabaseAsync( IHomeFlowDbContext context, IMapper mapper, CancellationToken cancellationToken )
    {
        try
        {
            // Create directories if they do not exist
            Directory.CreateDirectory( _baseBackupDir );
            Directory.CreateDirectory( _dbBaseDir );
            Directory.CreateDirectory( _dbBackupBaseDir );

            // export JSON data
            var exportData = await GetExportJsonData( context, mapper, cancellationToken );
            var memoryStream = new MemoryStream();

            // create the zip archive
            using ( var archive = new System.IO.Compression.ZipArchive( memoryStream, System.IO.Compression.ZipArchiveMode.Create, true ) )
            {
                // backup json export data
                foreach ( var entry in exportData )
                {
                    // Create a file in the ZIP archive
                    var fileName = $"{entry.Key}.json";
                    var fileEntry = archive.CreateEntry( fileName, System.IO.Compression.CompressionLevel.Optimal );

                    // Write the JSON content to the file
                    using var entryStream = fileEntry.Open();
                    using var streamWriter = new StreamWriter( entryStream );
                    await streamWriter.WriteAsync( entry.Value );
                }

                // Also, backup the image files from local storage
                var imageFiles = context.ImageFiles.ToList();
                foreach ( var imageFile in imageFiles )
                {
                    // get file name from path
                    var fileName = Path.GetFileName( imageFile.Path );
                    if ( string.IsNullOrEmpty( fileName ) )
                    {
                        continue;
                    }

                    // get the image file from localstorage/images
                    var imagePath = Path.Combine( _imageBaseDir, fileName );
                    if ( File.Exists( imagePath ) )
                    {
                        var fileEntry = archive.CreateEntry( Path.Combine( "ImageFiles", fileName ), System.IO.Compression.CompressionLevel.Optimal );

                        using var entryStream = fileEntry.Open();
                        using var fileStream = new FileStream( imagePath, FileMode.Open, FileAccess.Read );
                        await fileStream.CopyToAsync( entryStream );
                    }
                }

                // also, create a backup of the database file
                using ( var source = new SqliteConnection( $"Data Source={_dbBaseDir}/{_dbName}" ) )
                using ( var destination = new SqliteConnection( $"Data Source={_dbBackupBaseDir}/{_dbName}" ) )
                {
                    source.Open();
                    destination.Open();
                    source.BackupDatabase( destination );

                    // Force WAL flush and switch to DELETE journaling mode in the** backup**
                    using var pragmaCmd = destination.CreateCommand();
                    pragmaCmd.CommandText = @"
                    PRAGMA wal_checkpoint(FULL);
                    PRAGMA journal_mode=DELETE;
                ";
                    pragmaCmd.ExecuteNonQuery();

                    destination.Close();
                }

                // copy the DatabaseBackups folder to the localstorage folder
                var dbBackupDir = Path.Combine( "LocalStorage", "Database", "DatabaseBackups" );
                var tempBackupDir = Path.Combine( Path.GetTempPath(), $"HomeFlowBackup_{Guid.NewGuid()}" );

                Directory.CreateDirectory( tempBackupDir );

                // Copy all files to a temp directory
                foreach ( var fp in Directory.GetFiles( dbBackupDir, "*", SearchOption.AllDirectories ) )
                {
                    var relativePath = Path.GetRelativePath( dbBackupDir, fp );
                    var destPath = Path.Combine( tempBackupDir, relativePath );

                    Directory.CreateDirectory( Path.GetDirectoryName( destPath )! );
                    File.Copy( fp, destPath, overwrite: true );
                }

                // Add everything from the temp directory to the archive under DatabaseBackups/
                foreach ( var fp in Directory.GetFiles( tempBackupDir, "*", SearchOption.AllDirectories ) )
                {
                    var relativePath = Path.GetRelativePath( tempBackupDir, fp );
                    var entryPath = Path.Combine( "DatabaseBackups", relativePath );

                    var entry = archive.CreateEntry( entryPath, CompressionLevel.Optimal );
                    using var entryStream = entry.Open();
                    using var fileStream = new FileStream( fp, FileMode.Open, FileAccess.Read );
                    await fileStream.CopyToAsync( entryStream );
                }

                // Cleanup temp dir
                Directory.Delete( tempBackupDir, recursive: true );
            }

            memoryStream.Seek( 0, SeekOrigin.Begin );

            return memoryStream;
        }
        catch ( Exception )
        {
            throw;
        }
    }

    public static async Task<string> OptimizeDatabaseAsync( IHomeFlowDbContext context )
    {
        try
        {
            var imageIds = new List<Guid>();

            var recipeImageIds = await context.Recipes
                .Where( r => r.ImageId != null )
                .Select( r => r.ImageId!.Value )
                .ToListAsync();

            var familyMemberImageIds = await context.FamilyMembers
                .Where( r => r.ImageId != null )
                .Select( r => r.ImageId!.Value )
                .ToListAsync();

            imageIds.AddRange( recipeImageIds );
            imageIds.AddRange( familyMemberImageIds );

            // check for any image files in the file system that are not in the database
            var imageFiles = Directory.GetFiles( _imageBaseDir, "*.*", SearchOption.AllDirectories )
                .Select( Path.GetFileName )
                .ToList();

            var imageFilesToDelete = imageFiles
                .Where( fileName => !imageIds.Contains( Guid.Parse( Path.GetFileNameWithoutExtension( fileName )! ) ) )
                .ToList();

            int count = 0;

            foreach ( var fileName in imageFilesToDelete )
            {
                if ( fileName == null )
                {
                    continue;
                }

                var filePath = Path.Combine( _imageBaseDir, fileName );
                if ( File.Exists( filePath ) )
                {
                    File.Delete( filePath );
                    count++;
                }
            }

            // wait at least a second to give the ui some buffer since this task runs really fast.
            Thread.Sleep( 1000 );

            return $"Removed {count} image files.";
        }
        catch ( Exception ex )
        {
            return $"Error: {ex}";
        }
    }

    private static async Task<Dictionary<string, string>> GetExportJsonData( IHomeFlowDbContext context, IMapper mapper, CancellationToken cancellationToken )
    {
        var exportData = new Dictionary<string, string>();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // TODO: This should all be dynamic
        var recipes = await context.Recipes.ProjectTo<Recipe>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "recipes", JsonSerializer.Serialize( recipes, options ) );

        var groceryItems = await context.GroceryItems.ProjectTo<GroceryItem>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "groceryitems", JsonSerializer.Serialize( groceryItems, options ) );

        var groceryLists = await context.GroceryLists.ProjectTo<GroceryList>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "grocerylists", JsonSerializer.Serialize( groceryLists, options ) );

        var mealPlannerItems = await context.MealPlannerItems.ProjectTo<MealPlannerItem>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "mealplanneritems", JsonSerializer.Serialize( mealPlannerItems, options ) );

        var familyMembers = await context.FamilyMembers.ProjectTo<FamilyMember>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "familymembers", JsonSerializer.Serialize( familyMembers, options ) );

        var tags = await context.Tags.ProjectTo<Tag>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "tags", JsonSerializer.Serialize( tags, options ) );

        var systemSettings = await context.SystemSettings.ProjectTo<SystemSetting>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "systemsettings", JsonSerializer.Serialize( systemSettings, options ) );

        var imageFiles = await context.ImageFiles.ProjectTo<ImageFile>( mapper.ConfigurationProvider ).ToListAsync( cancellationToken );
        exportData.Add( "imagefiles", JsonSerializer.Serialize( imageFiles, options ) );

        return exportData;
    }

    public static async Task ImportEntitiesAsync<TContract, TEntity>(
            Dictionary<string, string> importData,
            string key,
            DbSet<TEntity> dbSet,
            Func<TContract, Task> importHandler,
            CancellationToken cancellationToken = default
        )
    where TEntity : class
    {
        if ( !importData.TryGetValue( key, out var json ) || string.IsNullOrWhiteSpace( json ) )
        {
            return;
        }

        var contracts = JsonSerializer.Deserialize<List<TContract>>( json );
        if ( contracts == null || !contracts.Any() )
        {
            return;
        }

        // Clear existing database entries (TEntity)
        await dbSet.ExecuteDeleteAsync( cancellationToken );

        // Import each contract (TContract)
        foreach ( var contract in contracts )
        {
            await importHandler( contract );
        }
    }
}
