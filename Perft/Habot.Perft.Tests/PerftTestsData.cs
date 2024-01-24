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


    private static readonly Fen Position4Fen = new("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");

    /// <summary>
    /// https://www.chessprogramming.org/Perft_Results#Position_4
    /// </summary>
    public static IEnumerable Position4
    {
        get
        {
            yield return new TestCaseData(Position4Fen, 1).Returns(6);
            yield return new TestCaseData(Position4Fen, 2).Returns(264);
            yield return new TestCaseData(Position4Fen, 3).Returns(9_467);
            yield return new TestCaseData(Position4Fen, 4).Returns(422_333);
            yield return new TestCaseData(Position4Fen, 5).Returns(15_833_292);
            // yield return new TestCaseData(Position4Fen, 6).Returns(706_045_033);
        }
    }

    private static readonly Fen Position5Fen = new("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8  ");

    /// <summary>
    /// https://www.chessprogramming.org/Perft_Results#Position_5
    /// </summary>
    public static IEnumerable Position5
    {
        get
        {
            yield return new TestCaseData(Position5Fen, 1).Returns(44);
            yield return new TestCaseData(Position5Fen, 2).Returns(1_486);
            yield return new TestCaseData(Position5Fen, 3).Returns(62_379);
            yield return new TestCaseData(Position5Fen, 4).Returns(2_103_487);
            // yield return new TestCaseData(Position5Fen, 5).Returns(89_941_194);
        }
    }

    private static readonly Fen Position6Fen =
        new("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");

    /// <summary>
    /// https://www.chessprogramming.org/Perft_Results#Position_6
    /// </summary>
    public static IEnumerable Position6
    {
        get
        {
            yield return new TestCaseData(Position6Fen, 1).Returns(46);
            yield return new TestCaseData(Position6Fen, 2).Returns(2_079);
            yield return new TestCaseData(Position6Fen, 3).Returns(89_890);
            yield return new TestCaseData(Position6Fen, 4).Returns(3_894_594);
            // yield return new TestCaseData(Position6Fen, 5).Returns( 164_075_551 );
        }
    }
}