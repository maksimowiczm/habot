using Habot.UCI.Notation;

namespace Habot.UCI.Request;

public readonly record struct PositionFromStartPos(
    IEnumerable<Move> Moves
) : IUciRequestWithMoves;