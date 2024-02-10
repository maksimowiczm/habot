using System.Runtime.Serialization;

namespace Habot.UCI.Notation;

public enum PieceType
{
    King,
    Queen,
    Rook,
    Bishop,
    Knight,
    Pawn
}

public static class PieceTypeExtensions
{
    public static PieceType ToPieceType(this char ch)
    {
        return char.ToLower(ch) switch
        {
            'k' => PieceType.King,
            'q' => PieceType.Queen,
            'r' => PieceType.Rook,
            'b' => PieceType.Bishop,
            'n' => PieceType.Knight,
            'p' => PieceType.Pawn,
            _ => throw new SerializationException($"'{ch}' is not a piece")
        };
    }
}