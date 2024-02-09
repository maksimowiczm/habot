using Habot.UCI.Notation;
using NUnit.Framework;

namespace Habot.Perft.Tests;

public abstract class PerftTests<TBoard>
    where TBoard : IPerftBoard
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

        var moves = board.Perft(depth).ToList();
        var count = moves.Aggregate(0, (current, m) => current + m.Count);

        foreach (var move in moves)
        {
            Console.WriteLine($"{move.Move}: {move.Count}");
        }

        return count;
    }

    protected abstract TBoard CreateBoard(Fen fen);
}