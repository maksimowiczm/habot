using Habot.Core.Chess;

namespace Habot.UCI.Request;

public readonly record struct PositionFromFen(
    Fen Fen,
    IEnumerable<Move> Moves
) : IUciRequestWithMoves;