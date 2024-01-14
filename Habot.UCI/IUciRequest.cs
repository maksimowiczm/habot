using Habot.Core.Board;

namespace Habot.UCI;

public interface IUciRequest
{
}

public interface IUciRequestWithMoves : IUciRequest
{
    IEnumerable<Move> Moves { get; }
}