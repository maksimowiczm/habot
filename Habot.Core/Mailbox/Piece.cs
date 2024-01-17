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