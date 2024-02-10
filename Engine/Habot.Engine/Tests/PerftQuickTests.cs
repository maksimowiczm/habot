using Habot.Engine.Board;
using Habot.Perft;
using Habot.Perft.Tests;
using Habot.UCI.Notation;
using NUnit.Framework;

namespace Habot.Engine.Tests;

[TestFixture]
public class PerftQuickTests : PerftQuickTests<PerftBoard<SmartBoard>>
{
    protected override PerftBoard<SmartBoard> CreateBoard(Fen fen)
    {
        var board = new BoardBuilder<SmartBoard>().SetFen(fen).Build();
        return new PerftBoard<SmartBoard>(board, board);
    }
}