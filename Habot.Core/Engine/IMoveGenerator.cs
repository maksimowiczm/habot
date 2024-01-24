using Habot.UCI.Notation;

namespace Habot.Core.Engine;

public interface IMoveGenerator
{
    IEnumerable<Move> GetLegalMoves();
}