namespace Habot.Core.Perft;

/// <summary>
/// Represents chess board that has a perft methods.
/// https://www.chessprogramming.org/Perft
/// </summary>
public interface IPerftBoard
{
    IEnumerable<PerftWithMove> Perft(uint depth);
    uint PerftQuick(uint depth);
}