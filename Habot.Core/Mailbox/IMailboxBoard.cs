using Habot.UCI.Notation;
using Shared;
using static Habot.UCI.Notation.Color;
using static Habot.Core.Mailbox.PieceType;

namespace Habot.Core.Mailbox;

public interface IMailboxBoard
{
    public static IEnumerable<(int, Piece)> StartingPositionPiecesMap
    {
        get
        {
            var piecesMap = new List<(int, Piece)>
            {
                (0, new Piece(Rook, White)),
                (1, new Piece(Knight, White)),
                (2, new Piece(Bishop, White)),
                (3, new Piece(Queen, White)),
                (4, new Piece(King, White)),
                (5, new Piece(Bishop, White)),
                (6, new Piece(Knight, White)),
                (7, new Piece(Rook, White)),

                (63, new Piece(Rook, Black)),
                (62, new Piece(Knight, Black)),
                (61, new Piece(Bishop, Black)),
                (60, new Piece(King, Black)),
                (59, new Piece(Queen, Black)),
                (58, new Piece(Bishop, Black)),
                (57, new Piece(Knight, Black)),
                (56, new Piece(Rook, Black)),
            };

            foreach (var i in Enumerable.Range(8, 8))
            {
                piecesMap.Add((i, new Piece(Pawn, White)));
            }

            foreach (var i in Enumerable.Range(48, 8))
            {
                piecesMap.Add((i, new Piece(Pawn, Black)));
            }

            return piecesMap;
        }
    }

    public static IEnumerable<(int, int, Piece)> PiecesFromFen(Fen fen)
    {
        var position = fen.Value.TakeWhile(ch => ch != ' ').CollectString();
        var lines = position.Split("/").Reverse().ToList();
        var piecesCoordinates = lines
            .Select((line, row) =>
            {
                var col = 0;
                return line
                    .Select(ch =>
                    {
                        var color = char.IsUpper(ch) ? White : Black;
                        (int, int, Piece)? piece = char.ToLower(ch) switch
                        {
                            'r' => (row, col, new Piece(Rook, color)),
                            'n' => (row, col, new Piece(Knight, color)),
                            'b' => (row, col, new Piece(Bishop, color)),
                            'q' => (row, col, new Piece(Queen, color)),
                            'k' => (row, col, new Piece(King, color)),
                            'p' => (row, col, new Piece(Pawn, color)),
                            _ => null
                        };

                        var step = piece is null ? ch - '0' : 1;
                        col += step;

                        return piece;
                    })
                    .Where(p => p is not null)
                    .Select(p => p!.Value)
                    .ToList();
            })
            .Where(l => l.Count > 0)
            .SelectMany(l => l)
            .ToList();

        return piecesCoordinates;
    }
}