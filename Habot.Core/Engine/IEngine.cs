using Habot.UCI.Notation;
using Habot.UCI.Request;

namespace Habot.Core.Engine;

public interface IEngine
{
    Move Search(Go request, IEvaluableBoard board);
}