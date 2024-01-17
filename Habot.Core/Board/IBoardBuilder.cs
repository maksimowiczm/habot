using Habot.UCI.Notation;

namespace Habot.Core.Board;

public interface IBoardBuilder<out T, out TSelf>
    where TSelf : IBoardBuilder<T, TSelf>
{
    TSelf SetFen(Fen fen);
    TSelf SetStartingPosition();
    T Build();
}