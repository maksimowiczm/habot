namespace Habot.Core.Board;

public interface ICreatableBoard<out TSelf>
    where TSelf : ICreatableBoard<TSelf>
{
    TSelf Create();
}