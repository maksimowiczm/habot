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
            yield return new TestCaseData(StartingPositionFen, 0).Returns(1);
            yield return new TestCaseData(StartingPositionFen, 1).Returns(20);
            yield return new TestCaseData(StartingPositionFen, 2).Returns(400);
            yield return new TestCaseData(StartingPositionFen, 3).Returns(8_902);
            yield return new TestCaseData(StartingPositionFen, 4).Returns(197_281);
            yield return new TestCaseData(StartingPositionFen, 5).Returns(4_865_609);
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
            yield return new TestCaseData(Position2Fen, 1).Returns(48);
            yield return new TestCaseData(Position2Fen, 2).Returns(2_039);
            yield return new TestCaseData(Position2Fen, 3).Returns(97_862);
            yield return new TestCaseData(Position2Fen, 4).Returns(4_085_603);
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
            yield return new TestCaseData(Position3Fen, 1).Returns(14);
            yield return new TestCaseData(Position3Fen, 2).Returns(191);
            yield return new TestCaseData(Position3Fen, 3).Returns(2_812);
            yield return new TestCaseData(Position3Fen, 4).Returns(43_238);
            yield return new TestCaseData(Position3Fen, 5).Returns(674_624);
            yield return new TestCaseData(Position3Fen, 6).Returns(11_030_083);
        }
    }
}