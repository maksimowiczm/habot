using Habot.Core.Board;

namespace Habot.UCI.Request;

public readonly record struct PositionFromFen(
    string Fen,
    IEnumerable<Move> Moves
) : IUciRequestWithMoves;