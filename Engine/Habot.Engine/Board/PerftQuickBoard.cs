using Habot.Perft;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class PerftQuickBoard : SmartBoard, IPerftQuickBoard
{
    public Fen Fen => ToFen();

    public int PerftQuick(int depth)
    {
        var movesCount = 0;
        var legalMoves = GetLegalMoves();
        if (depth == 0)
        {
            return 1;
        }

        if (depth == 1)
        {
            return legalMoves.Count();
        }

        foreach (var move in legalMoves)
        {
            Move(move);
            movesCount += PerftQuick(depth - 1);
            Undo();
        }

        return movesCount;
    }
}