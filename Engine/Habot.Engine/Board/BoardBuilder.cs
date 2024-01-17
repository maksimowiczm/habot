using Habot.Core.Board;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class BoardBuilder<T> : IBoardBuilder<T, BoardBuilder<T>>
    where T : Board, new()
{
    private readonly T _board = new();

    public BoardBuilder<T> SetFen(Fen fen)
    {
        _board.SetPosition(fen);
        return this;
    }

    public BoardBuilder<T> SetStartingPosition()
    {
        _board.SetStartingPosition();
        return this;
    }

    public T Build() => _board;
}