using Habot.Core.Board;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class SmartBoard : Board, ISmartBoard
{
    public IEnumerable<Move> GetLegalMoves(Color color)
    {
        throw new NotImplementedException();
    }
}