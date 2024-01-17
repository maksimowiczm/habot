using Habot.Core.Board;
using Habot.UCI.Notation;
using NUnit.Framework;

namespace Habot.Perft.Tests;

public abstract class PerftTests<TBoard>
    where TBoard : IPerftBoard, IBoard, ICreatableBoard<TBoard>
{
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.StartingPosition))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position2))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position3))]
    public uint PerftBoard(Fen fen, uint depth)
    {
        var board = TBoard.Create(fen);

        var moves = board.Perft(depth);
        var count = moves.Aggregate<PerftWithMove, uint>(0, (current, m) => current + m.Count);

        return count;
    }
}