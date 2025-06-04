namespace StaffApp.Application.DTOs.Common
{
    public class PaginatedResultDTO<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool IsReadOnly { get; set; }
    }
}
