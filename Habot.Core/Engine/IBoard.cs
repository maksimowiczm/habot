using Habot.UCI.Notation;

namespace Habot.Core.Engine;

public interface IBoard
{
    /// <summary>
    /// 8x8 chess board where index is a square number. 0 => a1 (0, 0), 63 => h8 (8, 8)
    /// </summary>
    IEnumerable<Piece?> Board { get; }

    IEnumerable<(Square, Piece)> Pieces => Board
        .Select((piece, square) => (piece, square))
        .Where(p => p.piece is not null)
        .Select(p => (new Square(p.square), p.piece!));

    Square? EnPassant { get; }

    CastleRights CastleRights { get; }

    Color ColorToMove { get; }
}