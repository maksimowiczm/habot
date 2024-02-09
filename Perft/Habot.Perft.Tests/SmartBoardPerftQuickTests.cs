using Habot.UCI.Notation;
using NUnit.Framework;

namespace Habot.Perft.Tests;

public abstract class PerftQuickTests<TBoard>
    where TBoard : IPerftQuickBoard
{
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.StartingPosition))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position2))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position3))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position4))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position5))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position6))]
    public int PerftBoard(Fen fen, int depth)
    {
        var board = CreateBoard(fen);

        var count = board.PerftQuick(depth);

        return count;
    }

    protected abstract TBoard CreateBoard(Fen fen);
}