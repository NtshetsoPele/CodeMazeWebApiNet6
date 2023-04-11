namespace Shared.RequestFeatures;

// Holds the common properties for all the
// entities in our project.
public abstract class RequestParameters
{
    // Restrict our API to a maximum of 50 rows per page
    private const byte MaxPageSize = 50;
    private byte _pageSize = 10;
    
    public int PageNumber { get; set; } = 1;
    public byte PageSize
    {
        get => _pageSize;
        set => _pageSize = 
            (value > MaxPageSize) ? MaxPageSize : value;
    }
}