using Habot.UCI.Notation;
using Habot.UCI.Request;

namespace Habot.Core.Engine;

public interface IEngine<in TGenerator, in TBoard>
    where TGenerator : IMoveGenerator
    where TBoard : IBoard
{
    Move Search(Go request, TGenerator generator, TBoard board);
}