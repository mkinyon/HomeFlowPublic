namespace HomeFlow.Services
{
    public interface IService<T>
    {
        Task<Guid> CreateAsync( T item );

        Task UpdateAsync( T item );

        Task DeleteAsync( Guid id );


        Task<T?> GetByIdAsync( Guid id );

        Task<List<T>> GetListAsync();

        Task<TableData<T>> GetTableDataAsync( QueryOptions options );
    }

    public class TableData<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
    }
}