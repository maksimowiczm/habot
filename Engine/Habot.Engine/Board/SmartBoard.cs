using System.Collections;
using Habot.Core.Board;
using Habot.Core.Mailbox;
using Habot.UCI.Notation;
using Shared;

namespace Habot.Engine.Board;

public class SmartBoard : Board, ISmartBoard
{
    public IEnumerable<Move> GetLegalMoves(Color color)
    {
        var attackedSquare = GetAttackedSquares(color.Toggle()).ToList();

        throw new NotImplementedException();
    }

    private bool IsPseudoLegal(Move move, Color color)
    {
        var piece = Pieces[move.To.Value];
        return piece is null || piece.Value.Color != color;
    }

    private IEnumerable<Square> GetAttackedByPawn(int position, Color color)
    {
        var attacked = PieceExtensions
            .GetStupidPawnMoves(color, new Square(position))
            .Last()
            .Where(m => IsPseudoLegal(m, color));

        return attacked.Select(m => m.To);
    }

    private IEnumerable<Square> GetAttackedSquares(Piece piece, int position, Color color)
    {
        if (piece.Type is PieceType.Pawn)
        {
            return GetAttackedByPawn(position, color);
        }

        var moves = piece.Type switch
        {
            PieceType.Pawn => throw new ArgumentException(),
            _ => piece.GetStupidMoves(new Square(position)),
        };

        var attacked = moves
            .SelectMany(flat => piece.Type switch
            {
                PieceType.Knight => flat.Where(m => IsPseudoLegal(m, color)),
                _ => flat.TakeWhile(m => IsPseudoLegal(m, color))
            });

        return attacked.Select(m => m.To).Distinct();
    }

    private IEnumerable<Square> GetAttackedSquares(Color color)
    {
        var pieces = Pieces
            .Select((piece, position) => new { piece, position })
            .Where(p => p.piece is not null && p.piece.Value.Color == color);

        var attackedSquares = pieces
            .Select(p => GetAttackedSquares(p.piece!.Value, p.position, color))
            .Flatten()
            .Distinct();

        return attackedSquares;
    }
}