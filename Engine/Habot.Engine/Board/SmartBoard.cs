using Habot.Core.Board;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class SmartBoard : Board, ISmartBoard, ICreatableBoard<SmartBoard>
{
    public IEnumerable<Move> GetLegalMoves(Color color)
    {
        throw new NotImplementedException();
    }

    public static SmartBoard Create() => PopulateProperties(new SmartBoard());

    public static SmartBoard Create(Fen fen) => PopulateProperties(new SmartBoard(), fen);
}