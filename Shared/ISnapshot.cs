namespace Shared;

public interface ISnapshot
{
    void Save();
    void Restore();
}