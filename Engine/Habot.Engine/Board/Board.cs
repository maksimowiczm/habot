using System.Runtime.Serialization;
using System.Text;
using Habot.Core.Board;
using Habot.Core.Mailbox;
using Habot.UCI.Notation;
using Shared;

namespace Habot.Engine.Board;

public class Board : IMailboxBoard, IBoard
{
    protected string CastleRights = "";
    public Color ColorToMove { get; protected set; } = Color.White;
    protected Square? EnPassant;
    protected Piece?[] Pieces = new Piece?[64];

    private void Clear()
    {
        foreach (var index in Enumerable.Range(0, 64))
        {
            Pieces[index] = null;
        }

        EnPassant = null;
        CastleRights = "";
    }

    public void SetStartingPosition()
    {
        Clear();

        foreach (var (key, piece) in IMailboxBoard.StartingPositionPiecesMap)
        {
            Pieces[key] = piece;
        }

        CastleRights = "KQkq";
    }

    public void SetPosition(Fen fen)
    {
        Clear();

        foreach (var (row, col, piece) in IMailboxBoard.PiecesFromFen(fen))
        {
            var flatSquare = new Square(row, col).Value;
            Pieces[flatSquare] = piece;
        }

        var options = fen.Value.SkipWhile(ch => ch != ' ').Skip(1).CollectString().Split(' ').Take(3).ToList();
        if (options.Count != 3)
        {
            throw new SerializationException($"""Cannot parse "{fen}" as fen""");
        }

        ColorToMove = options[0] == "w" ? Color.White : Color.Black;
        CastleRights = options[1];
        EnPassant = options[2] == "-" ? null : Square.Serialize(options[2]);
    }

    private bool TryCastle(int from, int to, Piece fromPiece)
    {
        if (fromPiece.Type != PieceType.King)
        {
            return false;
        }

        var color = fromPiece.Color;

        // move king
        (Pieces[to], Pieces[from]) = (Pieces[from], Pieces[to]);

        // move rook
        if (from < to)
        {
            var rookPosition = color == Color.White ? 7 : 63;
            (Pieces[from + 1], Pieces[rookPosition]) = (Pieces[rookPosition], Pieces[from + 1]);
            return true;
        }
        else // else bcs rookPosition var ;/
        {
            var rookPosition = color == Color.White ? 0 : 56;
            (Pieces[from - 1], Pieces[rookPosition]) = (Pieces[rookPosition], Pieces[from - 1]);
            return true;
        }
    }

    private bool TryEnPassant(int from, int to, Piece fromPiece)
    {
        if (
            fromPiece.Type != PieceType.Pawn ||
            EnPassant is null || EnPassant.Value.Value != to
        )
        {
            return false;
        }

        Pieces[to] = fromPiece;
        Pieces[from] = null;
        var diff = fromPiece.Color == Color.White ? -8 : 8;
        Pieces[EnPassant.Value.Value + diff] = null;
        return true;
    }

    private void NextMove(Square? enPassant = null)
    {
        EnPassant = enPassant;
        ColorToMove = ColorToMove.Toggle();
    }

    public virtual void Move(Move move)
    {
        var from = move.From;
        var to = move.To;
        var fromPiece = Pieces[from.Value];
        if (fromPiece is null)
        {
            return;
        }

        if (move.MightBeCastle() && TryCastle(from.Value, to.Value, fromPiece.Value))
        {
            NextMove();
            return;
        }

        if (TryEnPassant(from.Value, to.Value, fromPiece.Value))
        {
            NextMove();
            return;
        }

        var newPiece = move.Promotion switch
        {
            null => fromPiece.Value,
            var type => fromPiece.Value with { Type = type.Value.ToPieceType() }
        };

        Pieces[to.Value] = newPiece;
        Pieces[from.Value] = null;

        // mark en passant
        if (fromPiece.Value.Type == PieceType.Pawn && Math.Abs(from.Position.row - to.Position.row) == 2)
        {
            var diff = fromPiece.Value.Color == Color.White ? 8 : -8;
            var enPassant = new Square(from.Value + diff);
            NextMove(enPassant);
        }
        else
        {
            NextMove();
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        for (var i = 7; i >= 0; i--)
        {
            for (var j = 0; j < 8; j++)
            {
                var square = new Square(i, j);
                var piece = Pieces[square.Value];
                if (piece is not null)
                {
                    builder.Append(piece.ToString());
                }
                else
                {
                    builder.Append('.');
                }
            }

            builder.Append('\n');
        }

        return builder.ToString();
    }
}