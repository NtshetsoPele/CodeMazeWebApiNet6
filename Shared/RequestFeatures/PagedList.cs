namespace Shared.RequestFeatures;

public class PagedList<T> : List<T>
{
    public MetaData MetaData { get; set; }
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
        
        AddRange(items);
    }
    
    public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        int count = source.Count();
        IEnumerable<T> items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}