// using Habot.Engine.Board;
// using Habot.Perft;
// using Habot.Perft.Tests;
// using Habot.UCI.Notation;
// using NUnit.Framework;

// namespace Habot.Engine.MoveGenerator.Tests;

// [TestFixture]
// public class PerftTests : PerftTests<PerftBoard<MementoBoard>>
// {
//     protected override PerftBoard<MementoBoard> CreateBoard(Fen fen)
//     {
//         var board = new BoardBuilder<MementoBoard>().SetFen(fen).Build();
//         return new PerftBoard<MementoBoard>(board, new MoveGenerator());
//     }
// }