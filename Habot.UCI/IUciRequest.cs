using Habot.UCI.Notation;

namespace Habot.UCI;

public interface IUciRequest
{
}

public interface IUciPositionRequest : IUciRequest
{
    IEnumerable<Move> Moves { get; }
}