using Habot.UCI.Notation;

namespace Habot.Core.Board;

/// <summary>
/// Represents chess board that tells you which legal moves can be played.
/// </summary>
public interface ISmartBoard
{
    IEnumerable<Move> GetLegalMoves();
}