using Habot.UCI.Notation;

namespace Habot.UCI.Request;

public readonly record struct PositionFromFen(
    Fen Fen,
    IEnumerable<Move> Moves
) : IUciPositionRequest;