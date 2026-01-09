namespace HomeFlow.Infrastructure.FileStorage;

public interface IFileStorage
{
    Task<string> UploadAsync( Stream fileStream, string fileName, string folder, CancellationToken cancellationToken );

    Task<string> UpdateAsync( Stream fileStream, string filePath, CancellationToken cancellationToken );

    Task DeleteAsync( string filePath, CancellationToken cancellationToken );
}
