using Habot.Core.Board;
using Habot.UCI.Notation;
using NUnit.Framework;

namespace Habot.Perft.Tests;

public abstract class PerftTests<TBoard, TBuilder>
    where TBoard : IPerftBoard, IBoard
    where TBuilder : IBoardBuilder<TBoard, TBuilder>, new()
{
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.StartingPosition))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position2))]
    [TestCaseSource(typeof(PerftTestsData), nameof(PerftTestsData.Position3))]
    public int PerftBoard(Fen fen, int depth)
    {
        var board = new TBuilder().SetFen(fen).Build();

        var moves = board.Perft(depth);
        var count = moves.Aggregate(0, (current, m) => current + m.Count);

        return count;
    }
}