using Habot.UCI.Notation;

namespace Habot.Core.Board;

public interface IMementoBoard
{
    void Save(Move beforeMove);
    void Restore();
}