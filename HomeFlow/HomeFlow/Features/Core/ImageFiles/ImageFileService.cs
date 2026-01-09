
namespace HomeFlow.Features.Core.ImageFiles
{
    public interface IImageFileService
    {
        Task<ImageFile> UploadAsync( ImageFileRequest request );
        Task<ImageFile> UpdateAsync( Guid id, ImageFileRequest request );
    }

    public class ImageFileService : IImageFileService
    {
        private readonly ISender _sender;

        public ImageFileService( ISender sender )
        {
            _sender = sender;
        }

        Task<ImageFile> IImageFileService.UploadAsync( ImageFileRequest request ) =>
            _sender.Send( new UploadImageFileCommand( request ) );

        Task<ImageFile> IImageFileService.UpdateAsync( Guid id, ImageFileRequest request ) =>
            _sender.Send( new UpdateImageFileCommand( id, request ) );

    }
}
