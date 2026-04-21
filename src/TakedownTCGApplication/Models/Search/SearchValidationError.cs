namespace TakedownTCGApplication.Models.Search;

public sealed class SearchValidationError
{
    public SearchValidationError(string fieldName, string message)
    {
        FieldName = fieldName;
        Message = message;
    }

    public string FieldName { get; }
    public string Message { get; }
}
