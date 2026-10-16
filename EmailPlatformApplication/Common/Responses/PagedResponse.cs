namespace EmailPlatform.Application.Common.Responses;

public class PagedResponse<T>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalNumber { get; set; }

    public IEnumerable<T> Items { get; set; } = [];
}