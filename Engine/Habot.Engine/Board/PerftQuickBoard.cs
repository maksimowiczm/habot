using Habot.Core.Board;
using Habot.Perft;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class PerftQuickBoard : SmartBoard, IPerftQuickBoard
{
    public uint PerftQuick(uint depth)
    {
        throw new NotImplementedException();
    }
}