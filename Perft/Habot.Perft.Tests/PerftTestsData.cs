using System.Collections;
using Habot.UCI.Notation;
using NUnit.Framework;

namespace Habot.Perft.Tests;

public class PerftTestsData
{
    private static readonly Fen StartingPositionFen = new("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

    public static IEnumerable StartingPosition
    {
        get
        {
            yield return new TestCaseData(StartingPositionFen, 0u).Returns(1u);
            yield return new TestCaseData(StartingPositionFen, 1u).Returns(20u);
            yield return new TestCaseData(StartingPositionFen, 2u).Returns(400u);
            yield return new TestCaseData(StartingPositionFen, 3u).Returns(8_902u);
            yield return new TestCaseData(StartingPositionFen, 4u).Returns(197_281u);
            yield return new TestCaseData(StartingPositionFen, 5u).Returns(4_865_609u);
        }
    }


    private static readonly Fen Position2Fen = new("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");

    /// <summary>
    /// https://www.chessprogramming.org/Perft_Results#Position_2
    /// </summary>
    public static IEnumerable Position2
    {
        get
        {
            yield return new TestCaseData(Position2Fen, 1u).Returns(48u);
            yield return new TestCaseData(Position2Fen, 2u).Returns(2_039u);
            yield return new TestCaseData(Position2Fen, 3u).Returns(97_862u);
            yield return new TestCaseData(Position2Fen, 4u).Returns(4_085_603u);
        }
    }


    private static readonly Fen Position3Fen = new("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ");

    /// <summary>
    /// https://www.chessprogramming.org/Perft_Results#Position_3
    /// </summary>
    public static IEnumerable Position3
    {
        get
        {
            yield return new TestCaseData(Position3Fen, 1u).Returns(14u);
            yield return new TestCaseData(Position3Fen, 2u).Returns(191u);
            yield return new TestCaseData(Position3Fen, 3u).Returns(2_812u);
            yield return new TestCaseData(Position3Fen, 4u).Returns(43_238u);
            yield return new TestCaseData(Position3Fen, 5u).Returns(674_624u);
            yield return new TestCaseData(Position3Fen, 6u).Returns(11_030_083u);
        }
    }
}