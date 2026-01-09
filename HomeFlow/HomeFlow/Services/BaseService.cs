namespace HomeFlow.Services;

public abstract class BaseService<T> : IService<T>
{
    protected readonly IMediator _mediator;

    protected BaseService( IMediator mediator )
    {
        _mediator = mediator;
    }

    public abstract Task<Guid> CreateAsync( T item );
    public abstract Task UpdateAsync( T item );
    public abstract Task DeleteAsync( Guid id );

    public abstract Task<T?> GetByIdAsync( Guid id );
    public abstract Task<List<T>> GetListAsync();
    public abstract Task<TableData<T>> GetTableDataAsync( QueryOptions options );
}
