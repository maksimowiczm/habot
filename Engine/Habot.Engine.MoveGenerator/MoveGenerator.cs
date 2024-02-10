using Habot.Core.Engine;
using Habot.UCI.Notation;

namespace Habot.Engine.MoveGenerator;

// todo refactor this evil hell of methods in something fancy :)
public class MoveGenerator : IMoveGenerator
{
    public IEnumerable<Move> GetLegalMoves(IBoard board)
    {
        var position = new Position(board);
        return position.GetLegalMoves();
    }

    private class Position(IBoard board)
    {
        private readonly IBoard board = board;

        public IEnumerable<Move> GetLegalMoves()
        {
            throw new NotImplementedException();
        }
    }
}