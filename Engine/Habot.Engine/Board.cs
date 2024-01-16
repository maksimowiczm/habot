using System.Runtime.Serialization;
using System.Text;
using Habot.Core.Board;
using Habot.Core.Chess;
using Habot.Core.Mailbox;
using Habot.Perft;
using Shared;

namespace Habot.Engine;

public class Board : IMailboxBoard, IBoard, IPerftQuickBoard, ISmartBoard, ICreatableBoard<Board>
{
    private string _castleRights = "KQkq";
    private Color _colorToMove = Color.White;
    private Square? _enPassant;
    private Piece?[] _pieces = new Piece?[64];

    private Board()
    {
    }

    public static Board Create()
    {
        var board = new Board();

        foreach (var (key, value) in IMailboxBoard.StartingPositionPiecesMap)
        {
            board._pieces[key] = value;
        }

        return board;
    }

    public static Board Create(Fen fen)
    {
        var board = new Board();

        foreach (var (row, col, piece) in IMailboxBoard.PiecesFromFen(fen))
        {
            var flatSquare = new Square((byte)row, (byte)col).Value;
            board._pieces[flatSquare] = piece;
        }

        var options = fen.Value.SkipWhile(ch => ch != ' ').Skip(1).CollectString().Split(' ').Take(3).ToList();
        if (options.Count != 3)
        {
            throw new SerializationException($"""Cannot parse "{fen}" as fen""");
        }

        board._colorToMove = options[0] == "w" ? Color.White : Color.Black;
        board._castleRights = options[1];
        board._enPassant = options[2] == "-" ? null : Square.Serialize(options[2]);

        return board;
    }

    public void SetStartingPosition()
    {
        foreach (var (key, piece) in IMailboxBoard.StartingPositionPiecesMap)
        {
            _pieces[key] = piece;
        }
    }

    public void SetPosition(Fen fen)
    {
        var fenBoard = Create(fen);
        _pieces = fenBoard._pieces;
        _enPassant = fenBoard._enPassant;
        _castleRights = fenBoard._castleRights;
        _colorToMove = fenBoard._colorToMove;
    }

    public void Move(Move move)
    {
        throw new NotImplementedException();
    }

    public uint PerftQuick(uint depth)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Move> GetLegalMoves(Color color)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        for (var i = 7; i >= 0; i--)
        {
            for (var j = 0; j < 8; j++)
            {
                var square = new Square((byte)i, (byte)j);
                var piece = _pieces[square.Value];
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