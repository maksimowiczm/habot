using Habot.Engine.Board;
using Habot.Perft.Tests;
using NUnit.Framework;

namespace Habot.Engine.Tests;

[TestFixture]
public class PerftTests : PerftTests<PerftBoard, BoardBuilder<PerftBoard>>;