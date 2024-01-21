using Habot.UCI.Notation;

namespace Habot.Perft;

/// <summary>
/// Represents result of <see cref="IPerftBoard"/> preft method.
/// https://www.chessprogramming.org/Perft#Divide
/// </summary>
/// <param name="Move">Chess move</param>
/// <param name="Count">Count of legal moves after the move</param>
public readonly record struct PerftWithMove(
    Move Move,
    int Count
);