namespace HomeFlow.Infrastructure.FileStorage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _basePath = "LocalStorage/ImageFiles";

    public async Task<string> UploadAsync( Stream fileStream, string fileName, CancellationToken cancellationToken )
    {
        return await UploadAsync( fileStream, fileName, string.Empty, cancellationToken );
    }

    public async Task<string> UploadAsync( Stream fileStream, string fileName, string folder, CancellationToken cancellationToken )
    {
        // always make sure the stream is at the beginning
        fileStream.Position = 0;

        string fullPath = string.IsNullOrEmpty( folder ) ? _basePath : Path.Combine( _basePath, folder );
        string filePath = Path.Combine( fullPath, fileName );
        
        Directory.CreateDirectory( fullPath );

        using var file = File.Create( filePath );
        await fileStream.CopyToAsync( file, cancellationToken );

        return filePath;
    }

    public async Task<string> UpdateAsync( Stream fileStream, string filePath, CancellationToken cancellationToken = default )
    {
        // always make sure the stream is at the beginning
        fileStream.Position = 0;

        // Ensure the directory exists
        var directory = Path.GetDirectoryName( filePath );
        if ( !string.IsNullOrEmpty( directory ) )
        {
            Directory.CreateDirectory( directory );
        }

        // Write the new file content
        using var file = File.Create( filePath );
        await fileStream.CopyToAsync( file, cancellationToken );

        return filePath;
    }

    public Task DeleteAsync( string filePath, CancellationToken cancellationToken = default )
    {
        if ( File.Exists( filePath ) )
        {
            File.Delete( filePath );
        }
        return Task.CompletedTask;
    }
}
