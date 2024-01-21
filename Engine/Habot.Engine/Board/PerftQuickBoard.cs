using Habot.Core.Board;
using Habot.Core.Mailbox;
using Habot.Perft;
using Habot.UCI.Notation;
using Shared;

namespace Habot.Engine.Board;

public class PerftQuickBoard : SmartBoard, IPerftQuickBoard, ISnapshot
{
    public int PerftQuick(int depth)
    {
        var movesCount = 0;
        var legalMoves = GetLegalMoves(ColorToMove);
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

    public override void Move(Move move)
    {
        Save();
        base.Move(move);
    }

    public void Undo()
    {
        Restore();
    }

    private readonly record struct Snapshot(string CastleRights, Color ColorToMove, Square? EnPassant, Piece?[] Pieces);

    private readonly Stack<Snapshot> _snapshots = new();

    public void Save()
    {
        _snapshots.Push(new Snapshot(CastleRights, ColorToMove, EnPassant, (Piece?[])Pieces.Clone()));
    }

    public void Restore()
    {
        if (_snapshots.Count == 0)
        {
            return;
        }

        var snapshot = _snapshots.Pop();

        CastleRights = snapshot.CastleRights;
        ColorToMove = snapshot.ColorToMove;
        EnPassant = snapshot.EnPassant;
        Pieces = snapshot.Pieces;
    }
}