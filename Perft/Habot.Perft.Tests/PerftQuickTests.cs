using Habot.Core.Board;
using Habot.Core.Chess;
using NUnit.Framework;

namespace Habot.Perft.Tests;

public abstract class PerftQuickTests<TBoard>
    where TBoard : IPerftBoard, IBoard, ICreatableBoard<TBoard>
{
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.StartingPosition))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position2))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position3))]
    public uint PerftBoard(Fen fen, uint depth)
    {
        var board = TBoard.Create(fen);

        var count = board.PerftQuick(depth);

        return count;
    }
}