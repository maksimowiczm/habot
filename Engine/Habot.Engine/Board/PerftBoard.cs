using Habot.Perft;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class PerftBoard : SmartBoard, IPerftQuickBoard, IPerftBoard
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

    public IEnumerable<PerftWithMove> Perft(int depth)
    {
        // Special case have to return 1
        if (depth == 0)
        {
            return new List<PerftWithMove> { new(new Move(new Square(0), new Square(0)), 1) };
        }

        var legalMoves = GetLegalMoves();
        var list = new List<PerftWithMove>();
        foreach (var move in legalMoves)
        {
            Move(move);
            var movesCount = PerftQuick(depth - 1);
            Undo();
            list.Add(new PerftWithMove(move, movesCount));
        }

        return list;
    }
}