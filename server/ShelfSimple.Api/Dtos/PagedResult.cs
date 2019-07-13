using System.Collections.Generic;

namespace ShelfSimple.Api.Dtos
{
    public class PagedResult<T>
    {
        public IList<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
