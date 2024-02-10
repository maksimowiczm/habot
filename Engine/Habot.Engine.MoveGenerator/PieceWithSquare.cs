using Habot.UCI.Notation;

namespace Habot.Engine.MoveGenerator;

internal record PieceWithSquare(PieceType Type, Color Color, Square Position) : Piece(Type, Color)
{
    public static PieceWithSquare? Create(Piece? piece, Square position)
    {
        if (piece is null)
        {
            return null;
        }

        return new(piece.Type, piece.Color, position);
    }

    public IEnumerable<IEnumerable<Move>> GetStupidMoves()
    {
        var moves = PieceExtensions.GetStupidMoves(this, Position);

        if (Type is PieceType.Knight)
        {
            return moves.First().Select(m => new[] { m });
        }

        return moves;
    }
}