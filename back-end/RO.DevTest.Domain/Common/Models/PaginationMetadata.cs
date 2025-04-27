using System.Text.Json;
using System.Text.Json.Serialization;

namespace RO.DevTest.Domain.Common.Models
{
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }

        public PaginationMetadata(int currentPage, int totalPages, int pageSize, int totalCount)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalCount = totalCount;
            HasPrevious = currentPage > 1;
            HasNext = currentPage < totalPages;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

