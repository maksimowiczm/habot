using System.Text;
using Habot.Core;
using Habot.Core.Board;
using Habot.Core.Mailbox;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class MementoBoard : Board, IMementoBoard
{
    public override void Move(Move move)
    {
        Save(move);
        base.Move(move);
    }

    public void Undo()
    {
        Restore();
    }

    private readonly record struct Snapshot(
        CastleRights CastleRights,
        Color ColorToMove,
        Square? EnPassant,
        Piece?[] Pieces,
        int HalfMovesClock,
        int FullMoveClock,
        Move BeforeMove
    ) : IFenBoard
    {
        public Fen ToFen()
        {
            var fen = new StringBuilder();
            fen.Append(IMailboxBoard.PiecesToFen(Pieces));
            fen.Append($" {ColorToMove.ToFen()}");
            fen.Append($" {CastleRights}");
            fen.Append($" {EnPassant.EnPassantToFen()}");
            fen.Append($" {HalfMovesClock}");
            fen.Append($" {FullMoveClock}");
            return new Fen(fen.ToString());
        }

        public static Snapshot FromBoard(Board board, Move beforeMove) =>
            new(
                (CastleRights)board.CastleRights.Clone(),
                board.ColorToMove,
                board.EnPassant,
                (Piece?[])board.Pieces.Clone(),
                board.HalfMovesClock,
                board.FullMoveClock,
                beforeMove
            );

        public override string ToString() => ToFen().Value;
    }

    private readonly Stack<Snapshot> _snapshots = new();

    public void Save(Move beforeMove)
    {
        _snapshots.Push(Snapshot.FromBoard(this, beforeMove));
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
        HalfMovesClock = snapshot.HalfMovesClock;
        FullMoveClock = snapshot.FullMoveClock;
    }
}