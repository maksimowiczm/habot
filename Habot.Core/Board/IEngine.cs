using Habot.UCI.Notation;
using Habot.UCI.Request;

namespace Habot.Core.Board;

public interface IEngine
{
    Move Search(Go request);
}