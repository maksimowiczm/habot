using Habot.Core.Board;
using Habot.UCI.Notation;
using Habot.UCI.Request;

namespace Habot.Engine.Board;

public class Engine : PerftBoard, IEngine
{
    public Move Search(Go request)
    {
        throw new NotImplementedException();
    }
}