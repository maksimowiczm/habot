using System.Collections;
using Habot.Core.Board;
using Habot.Core.Mailbox;
using Habot.UCI.Notation;
using Shared;

namespace Habot.Engine.Board;

public class SmartBoard : Board, ISmartBoard
{
    /// <summary>
    /// Works only if position is legal.
    /// </summary>
    public IEnumerable<Move> GetLegalMoves(Color color)
    {
        var attackedSquare = GetAttackedSquares(color.Toggle()).ToList();
        var pinnedPieces = GetPins(color).ToList();

        throw new NotImplementedException();
    }

    /// <summary>
    /// Represents piece pin.
    /// </summary>
    /// <param name="Pinned">Pinned piece.</param>
    /// <param name="By">Piece that pins.</param>
    private readonly record struct Pin(Square Pinned, Square By);

    private IEnumerable<Pin> GetPins(Color color)
    {
        var kingPosition = new Square(
            Pieces
                .Select((p, i) => new { p, i })
                .Single(p => p.p is not null && p.p.Value.Type == PieceType.King && p.p.Value.Color == color)
                .i
        );

        var rookPins = GetPins(color, kingPosition, PieceType.Rook);
        var bishopPins = GetPins(color, kingPosition, PieceType.Bishop);

        return rookPins.Union(bishopPins);
    }

    private List<Pin> GetPins(Color color, Square kingPosition, PieceType byWho)
    {
        var possibleRookPinsPositions = new Piece(byWho, color)
            .GetStupidMoves(kingPosition)
            .Where(line => line.Any())
            .Select(line => line.Select(m => m.To));

        var pins = new List<Pin>();
        foreach (var line in possibleRookPinsPositions)
        {
            Square? friend = null;

            foreach (var square in line)
            {
                var piece = Pieces[square.Value];

                // skip empty square
                if (piece is null)
                {
                    continue;
                }

                // if piece is first friendly piece mark as friend
                if (piece.Value.Color == color && friend is null)
                {
                    friend = square;
                    continue;
                }

                // if piece enemy rook or queen mark as pinned and exit
                if (piece.Value.Color != color &&
                    (piece.Value.Type is PieceType.Queen || piece.Value.Type == byWho) &&
                    friend is not null)
                {
                    pins.Add(new Pin(friend.Value, square));
                }

                break;
            }
        }

        return pins;
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