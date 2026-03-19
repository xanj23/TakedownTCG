namespace TCGAPP
{
    public interface IQueryParams
    {
        Dictionary<string, QueryParam<object>> Parameters { get; }
    }
}