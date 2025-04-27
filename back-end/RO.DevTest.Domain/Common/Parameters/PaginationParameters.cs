namespace RO.DevTest.Domain.Common.Parameters // Updated namespace
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        /// <summary>
        /// Dynamic filter string using System.Linq.Dynamic.Core syntax.
        /// Example: "Name.Contains(\"John\") and Age > 30"
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Dynamic order by string using System.Linq.Dynamic.Core syntax.
        /// Example: "Name asc, Age desc"
        /// </summary>
        public string? OrderBy { get; set; }
    }
}

