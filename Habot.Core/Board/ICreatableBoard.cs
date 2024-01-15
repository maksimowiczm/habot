using Habot.Core.Chess;

namespace Habot.Core.Board;

public interface ICreatableBoard<out TSelf>
    where TSelf : ICreatableBoard<TSelf>
{
    static abstract TSelf Create();
    static abstract TSelf Create(Fen fen);
}