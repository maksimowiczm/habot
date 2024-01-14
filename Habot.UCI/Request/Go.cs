using Habot.Core.Board;

namespace Habot.UCI.Request;

public readonly record struct Go(
    IEnumerable<Move> Moves,
    bool Ponder,
    uint WhiteTime,
    uint BlackTime,
    uint WhiteIncrement,
    uint BlackIncrement,
    uint MovesToGo,
    uint Depth,
    uint Nodes,
    uint Mate,
    uint MoveTime,
    bool Infinite
) : IUciRequestWithMoves;