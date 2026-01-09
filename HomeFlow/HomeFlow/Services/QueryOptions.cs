namespace HomeFlow.Services
{
    public class QueryOptions
    {
        private string? _sortBy;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SearchTerm { get; set; } = string.Empty;

        public string? SortBy
        {
            get => (_sortBy ?? string.Empty).ToLower();
            set => _sortBy = value?.Trim();
        }

        public bool SortDescending { get; set; } = false;

        public Dictionary<string, string> AdditionalSettings { get; set; } = new();

        public QueryOptions() { }

        public QueryOptions( int page, int pageSize, string searchTerm, string sortBy, bool sortDescending = false )
        {
            Page = page;
            PageSize = pageSize;
            SearchTerm = searchTerm;
            SortBy = sortBy;
            SortDescending = sortDescending;
        }
    }
}
