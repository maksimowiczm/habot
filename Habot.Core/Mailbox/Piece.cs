using Habot.Core.Chess;

namespace Habot.Core.Mailbox;

public readonly record struct Piece
{
    public PieceType Type { get; private init; }
    public Color Color { get; private init; }

    public Piece(PieceType type, Color color)
    {
        Type = type;
        Color = color;
    }

    public enum PieceType
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }
}