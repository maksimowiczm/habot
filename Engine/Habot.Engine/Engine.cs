using Habot.Core.Board;
using Habot.Core.Engine;
using Habot.UCI.Notation;
using Habot.UCI.Request;

namespace Habot.Engine;

public class Engine<TGenerator, TBoard> : IEngine<TGenerator, TBoard>
    where TGenerator : IMoveGenerator
    where TBoard : IBoard, IMementoBoard, IPlayableBoard
{
    public Move Search(Go request, TGenerator moveGenerator, TBoard board)
    {
        throw new NotImplementedException();
    }
}