using Habot.Engine.Board;
using Habot.Perft.Tests;
using NUnit.Framework;

namespace Habot.Engine.Tests;

[TestFixture]
public class PerftQuickTests : PerftQuickTests<PerftBoard, BoardBuilder<PerftBoard>>;