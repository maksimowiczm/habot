using Habot.Core.Chess;

namespace Habot.Core.Board;

/// <summary>
/// Represents chess board on which you can move pieces around.
/// </summary>
public interface IBoard
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