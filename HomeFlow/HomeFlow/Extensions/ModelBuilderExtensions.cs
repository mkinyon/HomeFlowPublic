using Microsoft.EntityFrameworkCore.Metadata;

public static class ModelBuilderExtensions
{
    public static void UseEntityNameTableConventions( this ModelBuilder modelBuilder )
    {
        foreach ( IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes() )
        {
            var clrType = entityType.ClrType;
            if ( clrType == null )
                continue;

            if ( clrType.Name.EndsWith( "Entity" ) )
            {
                var cleanName = clrType.Name.Substring( 0, clrType.Name.Length - "Entity".Length );
                var tableName = Pluralize( cleanName );
                entityType.SetTableName( tableName );
            }
        }
    }

    // Optional pluralizer
    private static string Pluralize( string name )
    {
        // Simple rule — use a real pluralizer like Humanizer for production
        return name.EndsWith( "s" ) ? name : name + "s";
    }
}
