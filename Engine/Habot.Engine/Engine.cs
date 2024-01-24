using System.Collections.Immutable;
using Habot.Core.Board;
using Habot.Core.Engine;
using Habot.Core.Mailbox;
using Habot.UCI.Notation;
using Habot.UCI.Request;

namespace Habot.Engine;

public class Engine<TGenerator, TBoard> : IEngine<TGenerator, TBoard>
    where TGenerator : IMoveGenerator
    where TBoard : IBoard, IMementoBoard, IPlayableBoard
{
    private static double Value((Square, Piece) item)
    {
        var (square, piece) = item;

        return piece.Type switch
        {
            PieceType.King => 0,
            PieceType.Queen => 9,
            PieceType.Rook => 5,
            PieceType.Bishop => 3,
            PieceType.Knight => 3,
            PieceType.Pawn => 1,
        };
    }

    private double Evaluate(TBoard board, Color color)
    {
        var pieces = board.Pieces
            .GroupBy(g => g.Item2.Color)
            .ToImmutableDictionary(g => g.Key, g => g.ToImmutableList());

        return evaluate(color) - evaluate(color.Toggle());

        double evaluate(Color color) => pieces[color].Select(Value).Sum();
    }

    public Move Search(Go request, TGenerator moveGenerator, TBoard board)
    {
        var legalMoves = moveGenerator.GetLegalMoves().ToList();

        var evaluations = legalMoves
            .Select(move =>
            {
                board.Save(move);
                board.Move(move);

                // -evaluation due to color change after move
                var evaluation = -Evaluate(board, board.ColorToMove);
                board.Restore();
                return (move, evaluation);
            });

        var bestMove = evaluations.MaxBy(v => v.evaluation);

        return bestMove.move;
    }
}