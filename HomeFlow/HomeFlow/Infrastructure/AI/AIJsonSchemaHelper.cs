using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAi.JsonSchema.Generator;
using OpenAi.JsonSchema.Serialization;

namespace HomeFlow.Infrastructure.AI;

public static class AIJsonSchemaHelper
{
    public static BinaryData ConvertTo<T>( bool strict = true )
    {
        var jsonOptions = new JsonSerializerOptions( JsonSerializerDefaults.Web )
        {
            Converters = { new JsonStringEnumConverter( JsonNamingPolicy.SnakeCaseLower ) }
        };

        // use SchemaDefaults.OpenAi to enforce OpenAi rule set:
        var options = new JsonSchemaOptions( SchemaDefaults.OpenAi, jsonOptions );

        var resolver = new DefaultSchemaGenerator();
        var schema = resolver.Generate<T>( options );
        var json = schema.ToJson();

        return BinaryData.FromString( json );
    }

    public static T? ConvertFrom<T>( string jsonResponse )
    {
        // Use the same JSON options as the schema generation to handle enum conversion
        var jsonOptions = new JsonSerializerOptions( JsonSerializerDefaults.Web )
        {
            Converters = { new JsonStringEnumConverter( JsonNamingPolicy.SnakeCaseLower ) }
        };

        return JsonSerializer.Deserialize<T>( jsonResponse, jsonOptions );
    }
}
