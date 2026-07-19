namespace SplitRight.API.Models.Entities
{
    public class PagedResposneDto<T>
    {
        public List<T> Data { get; set; } = new List<T>();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount   { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => Page > 1;

        public bool HasNextpage => Page < TotalPages;


    }
}
