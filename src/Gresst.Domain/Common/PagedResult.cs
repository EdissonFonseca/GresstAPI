namespace Gresst.Domain.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
    }
}
