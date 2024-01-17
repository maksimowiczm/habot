using Habot.UCI.Notation;

namespace Habot.Core.Mailbox;

public readonly record struct Piece(PieceType Type, Color Color)
{
    public override string ToString()
    {
        var notation = Type switch
        {
            PieceType.King => "k",
            PieceType.Queen => "q",
            PieceType.Rook => "r",
            PieceType.Bishop => "b",
            PieceType.Knight => "n",
            PieceType.Pawn => "p",
            _ => throw new Exception("This is why does .NET enforce default branch for enums")
        };

        return Color == Color.White ? notation.ToUpper() : notation;
    }
}

public static class PieceExtensions
{
    /// <summary>
    /// This function returns all possible moves from a given position with a given piece.
    /// The moves are organized in an enumerable of enumerable of moves, grouped by the direction of the move.
    /// For example, a rook will have four possible directions (up, down, left, right), so it will return an enumerable of size 4.
    /// For pawns use <see cref="GetStupidPawnMoves"/> instead.
    /// </summary>
    /// <param name="piece">Piece.</param>
    /// <param name="position">Piece on board square.</param>
    /// <returns>Possible moves.</returns>
    public static IEnumerable<IEnumerable<Move>> GetStupidSlidingMoves(this Piece piece, Square position)
    {
        return piece.Type switch
        {
            PieceType.King => GetStupidQueenMoves(position, 1),
            PieceType.Queen => GetStupidQueenMoves(position),
            PieceType.Rook => GetStupidRookMoves(position),
            PieceType.Bishop => GetStupidBishopMoves(position),
            PieceType.Knight => GetStupidKnightMoves(position),
            PieceType.Pawn => GetStupidPawnMoves(piece.Color, position),
            _ => throw new Exception("This is why does .NET enforce default branch for enums")
        };
    }

    /// <summary>
    /// This function returns all possible piece moves from a given position.
    /// It also returns possible capture moves, it is up to caller to validate this moves.
    /// Don't forget about en passant
    /// </summary>
    /// <param name="color">Color of the pawn.</param>
    /// <param name="fromSquare">Position of the pawn.</param>
    /// <returns>Enumerable of moves where first is forward moves and second capture moves.</returns>
    public static IEnumerable<IEnumerable<Move>> GetStupidPawnMoves(Color color, Square fromSquare)
    {
        var (row, column) = fromSquare.Position;
        var forward = new List<Move>();

        switch (color)
        {
            case Color.White:
                if (row == 6)
                {
                    forward.Add(new Square(row + 1, column).ToMove(fromSquare, 'q'));
                    forward.Add(new Square(row + 1, column).ToMove(fromSquare, 'r'));
                    forward.Add(new Square(row + 1, column).ToMove(fromSquare, 'n'));
                    forward.Add(new Square(row + 1, column).ToMove(fromSquare, 'b'));
                }

                if (row < 6)
                {
                    forward.Add(new Square(row + 1, column).ToMove(fromSquare));
                }

                if (row == 1)
                {
                    forward.Add(new Square(row + 2, column).ToMove(fromSquare));
                }

                break;
            case Color.Black:
                if (row == 1)
                {
                    forward.Add(new Square(row - 1, column).ToMove(fromSquare, 'q'));
                    forward.Add(new Square(row - 1, column).ToMove(fromSquare, 'r'));
                    forward.Add(new Square(row - 1, column).ToMove(fromSquare, 'n'));
                    forward.Add(new Square(row - 1, column).ToMove(fromSquare, 'b'));
                }

                if (row > 1)
                {
                    forward.Add(new Square(row - 1, column).ToMove(fromSquare));
                }

                if (row == 6)
                {
                    forward.Add(new Square(row - 2, column).ToMove(fromSquare));
                }

                break;

            default:
                throw new Exception("This is why does .NET enforce default branch for enums");
        }

        var capture = new List<Move>();
        (int, int)[] allMoves = color switch
        {
            Color.White => [(row + 1, column - 1), (row + 1, column + 1),],
            Color.Black => [(row - 1, column - 1), (row - 1, column + 1),],
            _ => throw new Exception("This is why does .NET enforce default branch for enums")
        };
        capture.AddRange(allMoves.ToMovesInRange(fromSquare));

        return new List<IEnumerable<Move>> { forward, capture };
    }

    private static bool SquareInRange((int row, int column) square)
    {
        var (row, column) = square;
        return row is >= 0 and < 8 && column is >= 0 and < 8;
    }

    private static Move ToMove(this Square square, Square from, char? promotion = null) => new(from, square, promotion);

    private static IEnumerable<Move> ToMovesInRange(this IEnumerable<(int, int)> square, Square from) =>
        square
            .Where(SquareInRange)
            .Select(v => new Square(v))
            .Select(v => v.ToMove(from));

    private static IEnumerable<Move> ToMoves(this IEnumerable<Square> square, Square from) =>
        square.Select(v => v.ToMove(from));

    private static IEnumerable<IEnumerable<Move>> GetStupidKnightMoves(Square fromSquare)
    {
        var (row, column) = fromSquare.Position;

        (int, int)[] allMoves =
        [
            (row - 2, column - 1),
            (row - 1, column - 2),
            (row + 2, column - 1),
            (row + 1, column - 2),
            (row + 2, column + 1),
            (row + 1, column + 2),
            (row - 2, column + 1),
            (row - 1, column + 2),
        ];

        var moves = allMoves.ToMovesInRange(fromSquare);

        return new List<IEnumerable<Move>> { moves };
    }

    private static IEnumerable<IEnumerable<Move>> GetStupidQueenMoves(Square fromSquare, int range = 8) =>
        GetStupidBishopMoves(fromSquare, range).Union(GetStupidRookMoves(fromSquare, range));

    private static IEnumerable<IEnumerable<Move>> GetStupidBishopMoves(Square fromSquare, int range = 8)
    {
        var from = fromSquare.Value;
        var column = fromSquare.Position.column;

        var upperRight = Enumerable.Range(column + 1, 8 - column).Take(range)
            .Select(v => new Square(from + (v - column) * 9))
            .ToMoves(fromSquare);

        var bottomLeft = Enumerable.Range(1, column).Take(range)
            .Select(v => new Square(from - v * 9))
            .ToMoves(fromSquare);

        var upperLeft = Enumerable.Range(1, column).Take(range)
            .Select(v => new Square(from + v * 7))
            .ToMoves(fromSquare);

        var bottomRight = Enumerable.Range(column + 1, 8 - column).Take(range)
            .Select(v => new Square(from - (v - column) * 7))
            .ToMoves(fromSquare);

        return new List<IEnumerable<Move>> { upperRight, bottomRight, bottomLeft, upperLeft };
    }

    private static IEnumerable<IEnumerable<Move>> GetStupidRookMoves(Square fromSquare, int range = 8)
    {
        var (row, column) = fromSquare.Position;

        var left = Enumerable.Range(0, column).Reverse().Take(range)
            .Select(v => (row, v))
            .ToMovesInRange(fromSquare);

        var right = Enumerable.Range(column + 1, 8 - column).Take(range)
            .Select(v => (row, v))
            .ToMovesInRange(fromSquare);

        var up = Enumerable.Range(column + 1, 8 - column).Take(range)
            .Select(v => (v, column))
            .ToMovesInRange(fromSquare);

        var down = Enumerable.Range(0, column).Reverse().Take(range)
            .Select(v => (v, column))
            .ToMovesInRange(fromSquare);

        return new List<IEnumerable<Move>> { up, right, down, left };
    }
}