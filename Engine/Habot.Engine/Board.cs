using Habot.Core.Board;
using Habot.Core.Chess;
using Habot.Core.Mailbox;
using Habot.Perft;

namespace Habot.Engine;

public class Board : IMailboxBoard, IBoard, IPerftBoard, ISmartBoard, ICreatableBoard<Board>
{
    public void SetStartingPosition()
    {
        throw new NotImplementedException();
    }

    public void SetPosition(Fen fen)
    {
        throw new NotImplementedException();
    }

    public void Move(Move move)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PerftWithMove> Perft(uint depth)
    {
        throw new NotImplementedException();
    }

    public uint PerftQuick(uint depth)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Move> GetLegalMoves(Color color)
    {
        throw new NotImplementedException();
    }

    public static Board Create()
    {
        throw new NotImplementedException();
    }

    public static Board Create(Fen fen)
    {
        throw new NotImplementedException();
    }
}