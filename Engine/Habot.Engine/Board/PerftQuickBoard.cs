using Habot.Core.Board;
using Habot.Perft;
using Habot.UCI.Notation;

namespace Habot.Engine.Board;

public class PerftQuickBoard : SmartBoard, IPerftQuickBoard, ICreatableBoard<PerftQuickBoard>
{
    public uint PerftQuick(uint depth)
    {
        throw new NotImplementedException();
    }

    public new static PerftQuickBoard Create() => PopulateProperties(new PerftQuickBoard());

    public new static PerftQuickBoard Create(Fen fen) => PopulateProperties(new PerftQuickBoard(), fen);
}