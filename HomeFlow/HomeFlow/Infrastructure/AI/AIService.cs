using OpenAI.Chat;

namespace HomeFlow.Infrastructure.AI;

public class AIService : IAIService
{
    private readonly string _apiKey;
    private readonly string _modelName;
    private readonly ILogger<AIService> _logger;

    public AIService( IConfiguration configuration, ILogger<AIService> logger )
    {
        _logger = logger;
        _apiKey = configuration["OpenAI:ApiKey"] ?? "";
        _modelName = configuration["OpenAI:ModelName"] ?? "";
    }

    public async Task<string> GetResponse( string prompt )
    {
        throw new NotImplementedException();
    }

    public async Task<T?> GetStructuredResponse<T>( string prompt )
    {
        List<ChatMessage> messages = new();
        ChatMessage chatMessage = prompt;
        messages.Add( chatMessage );

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "structured_response_schema",
                jsonSchema: AIJsonSchemaHelper.ConvertTo<T>(),
                jsonSchemaIsStrict: true )
        };

        var client = new ChatClient( _modelName, _apiKey );
        var completion = await client.CompleteChatAsync( messages, options );

        if ( completion.Value?.Content?[0]?.Text != null )
        {
            var jsonResponse = completion.Value.Content[0].Text;
            return AIJsonSchemaHelper.ConvertFrom<T>( jsonResponse ) ?? throw new InvalidOperationException( "Failed to parse structured response." );
        }

        return default;
    }
}
