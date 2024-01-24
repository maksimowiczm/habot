using Habot.UCI.Notation;

namespace Habot.Core.Board;

/// <summary>
/// Represents chess board on which you can move pieces around.
/// </summary>
public interface IPlayableBoard
{
    /// <summary>
    /// Sets starting position.
    /// </summary>
    void SetStartingPosition();

    /// <summary>
    /// Sets position from fen notation.
    /// </summary>
    void SetPosition(Fen fen);

    void Move(Move move);
}