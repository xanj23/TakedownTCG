namespace TakedownTCG.Core.Abstractions;

public interface IJustTcgResponseMapper
{
    string Map(object responseData);
}
