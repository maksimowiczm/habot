using Habot.Core.Board;
using Habot.UCI.Notation;
using Habot.UCI.Request;

namespace Habot.Engine.Board;

public class Engine : SmartBoard, IEngine, ICreatableBoard<Engine>
{
    public Move Search(Go request)
    {
        throw new NotImplementedException();
    }

    public new static Engine Create() => PopulateProperties(new Engine());

    public new static Engine Create(Fen fen) => PopulateProperties(new Engine(), fen);
}