namespace HomeFlow.Infrastructure.AI;

public interface IAIService
{
    Task<string> GetResponse( string prompt );
    Task<T?> GetStructuredResponse<T>( string prompt );
}
