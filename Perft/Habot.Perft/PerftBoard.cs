using Habot.Core.Board;
using Habot.Core.Engine;
using Habot.UCI.Notation;

namespace Habot.Perft;

public class PerftBoard<T>(T board) : IPerftQuickBoard, IPerftBoard
where T : IMementoBoard, IPlayableBoard, IMoveGenerator
{
    private readonly T _board = board;

    public int PerftQuick(int depth)
    {
        var movesCount = 0;
        if (depth == 0)
        {
            return 1;
        }

        var legalMoves = _board.GetLegalMoves();
        if (depth == 1)
        {
            return legalMoves.Count();
        }

        foreach (var move in legalMoves)
        {
            _board.Move(move);
            movesCount += PerftQuick(depth - 1);
            _board.Restore();
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

        var legalMoves = _board.GetLegalMoves();
        var list = new List<PerftWithMove>();
        foreach (var move in legalMoves)
        {
            _board.Move(move);
            var movesCount = PerftQuick(depth - 1);
            _board.Restore();
            list.Add(new PerftWithMove(move, movesCount));
        }

        return list;
    }
}