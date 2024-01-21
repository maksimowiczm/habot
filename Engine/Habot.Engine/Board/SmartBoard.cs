using Habot.Core.Board;
using Habot.Core.Mailbox;
using Habot.UCI.Notation;
using Shared;

namespace Habot.Engine.Board;

public class SmartBoard : MementoBoard, ISmartBoard
{
    /// <summary>
    /// Works only if position is legal.
    /// </summary>
    public IEnumerable<Move> GetLegalMoves()
    {
        var color = ColorToMove;

        var pseudoMoves = GetPseudoMoves(color);

        // Remove illegal king moves
        var kingPosition = GetKingPosition(color);
        var attackedSquares = GetAttackedSquares(color.Toggle()).ToList();

        // if king is attacked
        if (attackedSquares.Any(s => s == kingPosition))
        {
            pseudoMoves = pseudoMoves.ToList();
            var kingMoves = GetKingSafeMoves(color, kingPosition, pseudoMoves, attackedSquares);
            var protectionMoves = GetKingProtectionMoves(color, kingPosition, pseudoMoves);
            return kingMoves.Union(protectionMoves);
        }

        pseudoMoves = pseudoMoves
            .Where(m => m.From != kingPosition || attackedSquares.All(a => a != m.To))
            .ToList();

        // Remove illegal moves with pinned pieces
        var pinnedPieces = GetPins(color).ToList();
        var illegalPins = pseudoMoves
            .Join(pinnedPieces,
                move => move.From,
                pin => pin.Pinned,
                (move, pin) => (move, pin)
            )
            .Where(tuple =>
            {
                var (move, pin) = tuple;

                // pinned piece can move on the same line as the pinner (pinned by)
                // rook - the same row or column
                if (pin.Type == PinType.Rook)
                {
                    if (move.From.Position.row == pin.By.Position.row)
                    {
                        return move.To.Position.row != move.From.Position.row;
                    }

                    return move.To.Position.column != move.From.Position.column;
                }

                // bishop - move has to be on the same diagonal
                if (pin.Type == PinType.Bishop)
                {
                    return !move.To.IsSameDiagonal(pin.By);
                }

                return true;
            })
            .Select(tuple => tuple.move)
            .ToList();

        pseudoMoves = pseudoMoves.Where(m => illegalPins.All(illegal => illegal != m)).ToList();

        // todo castling, check moves

        return pseudoMoves;
    }

    private IEnumerable<Move> GetKingSafeMoves(
        Color color,
        Square kingPosition,
        IEnumerable<Move> pseudoMoves,
        IEnumerable<Square> attackedSquares)
    {
        color = color.Toggle();

        var enemies = Pieces
            .Select((piece, position) => (piece, position))
            .Where(p => p.piece is not null && p.piece.Value.Color == color);

        var xrayLines = enemies
            .Select(p =>
            {
                var (piece, position) = p;

                if (piece!.Value.Type is PieceType.Bishop or PieceType.Queen)
                {
                    var diagonal = new Square(position).GetDiagonals();
                    var line = diagonal.FirstOrDefault(l => l.Any(s => s == kingPosition));
                    if (line is null)
                    {
                        return line;
                    }

                    var list = line.ToList();
                    list.Remove(new Square(position));

                    if (piece!.Value.Type is PieceType.Bishop)
                    {
                        return list;
                    }
                }

                if (piece!.Value.Type is PieceType.Rook or PieceType.Queen)
                {
                    var lines = new Square(position).GetRookLines();
                    var line = lines.FirstOrDefault(l => l.Any(s => s == kingPosition));
                    if (line is null)
                    {
                        return line;
                    }

                    var list = line.ToList();
                    list.Remove(new Square(position));
                    return list;
                }

                return null;
            })
            .WhereNotNull()
            .Flatten()
            .ToList();

        var kingMoves = pseudoMoves
            .Where(m => m.From == kingPosition && xrayLines.All(s => s != m.To) && attackedSquares.All(s => s != m.To))
            .ToList();

        return kingMoves;
    }

    private IEnumerable<Move> GetKingProtectionMoves(Color color, Square kingPosition, IEnumerable<Move> pseudoMoves)
    {
        color = color.Toggle();

        var enemies = Pieces
            .Select((piece, position) => (piece, position))
            .Where(p => p.piece is not null && p.piece.Value.Color == color);

        var attackedSquares = enemies
            .Select(p =>
            {
                var (piece, position) = p;
                if (piece!.Value.Type == PieceType.Knight)
                {
                    return null;
                }

                var attackedSquares = GetAttackedSquares(piece!.Value, position, color).ToList();

                var line = attackedSquares.FirstOrDefault(line => line.Any(s => s == kingPosition));

                if (line is null)
                {
                    return null;
                }

                var listLine = line.ToList();
                listLine.Add(new Square(position));
                return listLine;
            })
            .WhereNotNull()
            .IntersectMultiple()
            .Where(s => s != kingPosition);

        var pinnedPieces = GetPins(color.Toggle());

        var legalMoves = pseudoMoves
            .Where(m =>
                m.From != kingPosition &&
                attackedSquares.Any(s => s == m.To) &&
                pinnedPieces.All(pin => pin.Pinned != m.From)
            );

        return legalMoves;
    }

    /// <summary>
    /// Validate if given move is valid. Makes move and validates board. Really slow. 
    /// </summary>
    private bool ValidMove(Move move)
    {
        Move(move);
        var king = GetKingPosition(ColorToMove.Toggle());
        var attacked = GetAttackedSquares(ColorToMove).ToList();
        Restore();
        return attacked.All(s => s != king);
    }

    private IEnumerable<Move> GetPseudoMoves(Color color)
    {
        var pieces = Pieces
            .Select((piece, position) => new { piece, position })
            .Where(p => p.piece is not null && p.piece.Value.Color == color)
            .ToList();

        var moves = pieces
            .Select(p =>
                {
                    var moves = p.piece!.Value.GetStupidMoves(new Square(p.position));

                    if (p.piece.Value.Type == PieceType.Pawn)
                    {
                        var forward = moves.First().TakeWhile(m => Pieces[m.To.Value] is null);
                        var enPassant = moves.Last().Where(m => m.To == EnPassant && ValidMove(m));
                        var capture = moves.Last()
                            .Where(m =>
                                Pieces[m.To.Value] is not null &&
                                Pieces[m.To.Value]!.Value.Color != p.piece.Value.Color
                            );
                        return forward.Union(enPassant).Union(capture);
                    }

                    var legal = moves
                        .Select(line =>
                        {
                            if (p.piece.Value.Type == PieceType.Knight)
                            {
                                return line.Where(m => IsPseudoLegal(m, color));
                            }

                            var scanner = line.TakeWhile(m => IsPseudoLegal(m, color)).ToList();

                            // if capture then stop at capture
                            if (scanner.Any(m =>
                                    Pieces[m.To.Value] is not null && Pieces[m.To.Value]!.Value.Color != color))
                            {
                                return scanner.TakeWhileInclusive(m => Pieces[m.To.Value] is null);
                            }

                            return scanner;
                        })
                        .Flatten();

                    return legal;
                }
            )
            .Flatten()
            .ToList();

        return moves;
    }

    private Square GetKingPosition(Color color)
    {
        var kingPosition = new Square(
            Pieces
                .Select((p, i) => new { p, i })
                .Single(p => p.p is not null && p.p.Value.Type == PieceType.King && p.p.Value.Color == color)
                .i
        );

        return kingPosition;
    }

    /// <summary>
    /// Represents piece pin.
    /// </summary>
    /// <param name="Pinned">Pinned piece.</param>
    /// <param name="By">Piece that pins.</param>
    private readonly record struct Pin(Square Pinned, Square By, PinType Type);

    private enum PinType
    {
        Rook,
        Bishop
    }

    private IEnumerable<Pin> GetPins(Color color)
    {
        var kingPosition = GetKingPosition(color);

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
                    var type = byWho switch
                    {
                        PieceType.Rook => PinType.Rook,
                        PieceType.Bishop => PinType.Bishop,
                        _ => throw new ArgumentOutOfRangeException(nameof(byWho), byWho, null)
                    };
                    pins.Add(new Pin(friend.Value, square, type));
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
            .Last();

        return attacked.Select(m => m.To);
    }

    /// <summary>
    /// Get attacked squares by piece
    /// </summary>
    private IEnumerable<IEnumerable<Square>> GetAttackedSquares(Piece piece, int position, Color color)
    {
        if (piece.Type is PieceType.Pawn)
        {
            return new List<IEnumerable<Square>> { GetAttackedByPawn(position, color) };
        }

        var moves = piece.Type switch
        {
            PieceType.Pawn => throw new ArgumentException(),
            _ => piece.GetStupidMoves(new Square(position)),
        };

        var attacked = moves
            .Select(line =>
            {
                if (piece.Type == PieceType.Knight)
                {
                    return line;
                }

                return line.TakeWhileInclusive(m => Pieces[m.To.Value] is null);
            });

        return attacked.Select(line => line.Select(m => m.To)).Distinct();
    }

    /// <summary>
    /// Get all attacked squares by piece
    /// </summary>
    private IEnumerable<Square> GetAttackedSquares(Color color)
    {
        var pieces = Pieces
            .Select((piece, position) => new { piece, position })
            .Where(p => p.piece is not null && p.piece.Value.Color == color);

        var attackedSquares = pieces
            .Select(p => GetAttackedSquares(p.piece!.Value, p.position, color))
            .Flatten()
            .Flatten()
            .Distinct();

        return attackedSquares;
    }
}