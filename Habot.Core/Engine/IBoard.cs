using Habot.Core.Mailbox;
using Habot.UCI.Notation;

namespace Habot.Core.Engine;

public interface IBoard
{
    IEnumerable<(Square, Piece)> Pieces { get; }

    Color ColorToMove { get; }
}