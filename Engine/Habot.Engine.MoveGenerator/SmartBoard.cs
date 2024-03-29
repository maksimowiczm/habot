using Habot.Core.Engine;
using Habot.Core.Mailbox;
using Habot.Engine.Board;
using Habot.UCI.Notation;
using Shared;

namespace Habot.Engine.MoveGenerator;

// todo refactor this evil hell of methods in something fancy :)
public class SmartBoard : MementoBoard, IMoveGenerator, IEvaluableBoard
{
    /// <summary>
    /// Works only if position is legal.
    /// </summary>
    public IEnumerable<Move> GetLegalMoves(IBoard board)
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

        // remove illegal king moves
        pseudoMoves = pseudoMoves
            .Where(m => m.From != kingPosition || attackedSquares.All(a => a != m.To))
            .ToList();

        pseudoMoves = RemovePins(pseudoMoves, kingPosition);

        return pseudoMoves.Union(GetCastles(attackedSquares));
    }

    /// <summary>
    /// Removes illegal moves of pinned pieces
    /// </summary>
    private List<Move> RemovePins(List<Move> pseudoMoves, Square kingPosition)
    {
        // Remove illegal moves with pinned pieces
        var pinnedPieces = GetPins(ColorToMove).ToList();
        // Enumerable of illegal moves by pinned pieces
        var illegalMoves = pseudoMoves
            .Join(pinnedPieces,
                move => move.From,
                pin => pin.Pinned,
                (move, pin) => (move, pin)
            )
            .Where(tuple =>
            {
                var (move, pin) = tuple;
                var (_, pinner, _) = pin;
                var (pinnerPiece, pinnerPosition) = pinner;

                var line = pinnerPiece
                    .GetStupidMoves(pinnerPosition)
                    .Where(l => l.Any(m => m.To == kingPosition))
                    .Flatten()
                    .Select(m => m.To)
                    .SkipLast(1)
                    .Append(pinnerPosition)
                    .ToList();

                return line.All(s => s != move.To);
            })
            .Select(tuple => tuple.move);

        return pseudoMoves.Where(m => illegalMoves.All(illegal => illegal != m)).ToList();
    }

    /// <summary>
    /// Gets legal castles. Does not check if king is on check.
    /// </summary>
    private IEnumerable<Move> GetCastles(IEnumerable<Square> attackedSquares)
    {
        // [castle] = ([hasToBeSafe], [hasToBeEmpty])
        var squareThatHaveToBeSafeToCastle = new Dictionary<Castle, (IEnumerable<int>, IEnumerable<int>)>
        {
            [Castle.WhiteKing] = ([5, 6], [5, 6]),
            [Castle.WhiteQueen] = ([2, 3], [1, 2, 3]),
            [Castle.BlackKing] = ([61, 62], [61, 62]),
            [Castle.BlackQueen] = ([58, 59], [57, 58, 59])
        };
        var colorCastles = ColorToMove.Castles();

        var possibleCastles =
            squareThatHaveToBeSafeToCastle
                .Where(p =>
                    // match castle colors
                    colorCastles.Any(c => c == p.Key) &&
                    // can do this castle
                    CastleRights.Has(p.Key) &&
                    // none of the squares is attacked
                    p.Value.Item1.All(v => attackedSquares.All(s => s != new Square(v))) &&
                    // none piece between king and rook
                    p.Value.Item2.All(v => Board[v] is null)
                )
                .Select(p => p.Key);

        return possibleCastles.Select(c => c.ToMove());
    }

    private List<Square>? GetUnsafeSquares(Piece piece, Square position, Square kingPosition)
    {
        // consider only sliding pieces
        if (piece.Type is not (PieceType.Queen or PieceType.Rook or PieceType.Bishop))
        {
            return null;
        }

        var lines = piece.GetStupidMoves(position);

        // get line with king
        var line = lines
            .Select(l => l.Select(m => m.To))
            .FirstOrDefault(l => l.Any(s => s == kingPosition));

        if (line is null)
        {
            return null;
        }

        var attackedLine = line.ToList();

        var attackedSquares = new List<Square>();
        foreach (var square in attackedLine)
        {
            var pieceOnSquare = Board[square.Value];

            // skip empty squares
            if (pieceOnSquare is null)
            {
                attackedSquares.Add(square);
                continue;
            }

            // if there is piece on attacked square it means that king is hidden behind it so it is not attacked
            if (square != kingPosition)
            {
                return null;
            }

            // if king is on this square it means that it can not move on this line at all
            attackedSquares = attackedLine;
            break;
        }

        return attackedSquares;
    }

    /// <summary>
    /// Get legal king moves after check.
    /// </summary>
    private IEnumerable<Move> GetKingSafeMoves(
        Color color,
        Square kingPosition,
        IEnumerable<Move> pseudoMoves,
        IEnumerable<Square> attackedSquares
    )
    {
        var enemies = Board
            .Select((piece, position) => (piece, position))
            .Where(p => p.piece is not null && p.piece.Color == color.Toggle())
            .Select(p => (p.piece!, p.position));

        var unsafeSquares = enemies
            .Select(p => GetUnsafeSquares(p.Item1, new Square(p.position), kingPosition))
            .WhereNotNull()
            .Flatten()
            .ToList();

        // remove illegal king moves
        var kingMoves = pseudoMoves
            .Where(m => m.From == kingPosition &&
                        unsafeSquares.All(s => s != m.To) &&
                        attackedSquares.All(s => s != m.To)
            );

        return kingMoves;
    }

    /// <summary>
    /// Get moves that can protect king after check.
    /// </summary>
    private List<Move> GetKingProtectionMoves(Color color, Square kingPosition,
        IEnumerable<Move> pseudoMoves)
    {
        color = color.Toggle();
        pseudoMoves = pseudoMoves.ToList();

        var enemies = Board
            .Select((piece, position) => (piece, position))
            .Where(p => p.piece is not null && p.piece.Color == color)
            .Select(p => (p.piece!, p.position));

        var attackedSquares = enemies
            .Select<(Piece, int), (Piece, List<Square>)?>(p =>
            {
                var (piece, position) = p;
                var attackedSquares = GetAttackedSquares(piece, position, color).ToList();
                var line = attackedSquares.FirstOrDefault(line => line.Any(s => s == kingPosition));
                if (line is null)
                {
                    return null;
                }

                if (piece.Type == PieceType.Pawn)
                {
                    return (piece, new List<Square> { new(position) });
                }

                if (piece.Type == PieceType.Knight)
                {
                    if (attackedSquares.First().Any(s => s == kingPosition))
                    {
                        return (piece, new List<Square> { new(position), kingPosition });
                    }

                    return null;
                }


                var listLine = line.ToList();
                listLine.Add(new Square(position));
                return (piece, listLine);
            })
            .WhereNotNull()
            .ToList();

        var pinnedPieces = GetPins(color.Toggle());

        var legalMoves = pseudoMoves
            .Where(m =>
                m.From != kingPosition &&
                attackedSquares
                    .Select(x => x!.Value.Item2)
                    .IntersectMultiple()
                    .Where(s => s != kingPosition)
                    .Any(s => s == m.To) &&
                pinnedPieces.All(pin => pin.Pinned != m.From)
            )
            .ToList();

        if (EnPassant is null)
        {
            return legalMoves;
        }
        // add en passant if exists

        var attackerPawn = attackedSquares.SingleOrDefault(x => x!.Value.Item1.Type == PieceType.Pawn);
        if (attackerPawn is null)
        {
            return legalMoves;
        }

        var pawnMoves = pseudoMoves.Where(m => Board[m.From.Value]?.Type is PieceType.Pawn);
        var enPassants = pawnMoves.Where(m => m.To == EnPassant);
        legalMoves.AddRange(enPassants);

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

    private List<Move> GetPseudoMoves(Color color)
    {
        var pieces = Board
            .Select((piece, position) => new { piece, position })
            .Where(p => p.piece is not null && p.piece.Color == color)
            .ToList();

        var moves = pieces
            .Select(p =>
                {
                    var moves = p.piece!.GetStupidMoves(new Square(p.position)).ToList();

                    if (p.piece!.Type == PieceType.Pawn)
                    {
                        var forward = moves.First().TakeWhile(m => Board[m.To.Value] is null);
                        var enPassant = moves.Last().Where(m => m.To == EnPassant && ValidMove(m));
                        var capture = moves.Last()
                            .Where(m =>
                                Board[m.To.Value] is not null &&
                                Board[m.To.Value]!.Color != p.piece.Color
                            );
                        return forward.Union(enPassant).Union(capture);
                    }

                    var legal = moves
                        .Select(line =>
                        {
                            if (p.piece.Type == PieceType.Knight)
                            {
                                return line.Where(m => IsPseudoLegal(m, color));
                            }

                            var scanner = line.TakeWhile(m => IsPseudoLegal(m, color)).ToList();

                            // if capture then stop at capture
                            if (scanner.Any(m =>
                                    Board[m.To.Value] is not null && Board[m.To.Value]!.Color != color))
                            {
                                return scanner.TakeWhileInclusive(m => Board[m.To.Value] is null);
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
            Board
                .Select((p, i) => new { p, i })
                .Single(p => p.p is not null && p.p.Type == PieceType.King && p.p.Color == color)
                .i
        );

        return kingPosition;
    }

    /// <summary>
    /// Represents piece pin.
    /// </summary>
    /// <param name="Pinned">Pinned piece.</param>
    /// <param name="By">Piece that pins.</param>
    private readonly record struct Pin(Square Pinned, (Piece, Square) By, PinType Type);

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
                var piece = Board[square.Value];

                // skip empty square
                if (piece is null)
                {
                    continue;
                }

                // if piece is first friendly piece mark as friend
                if (piece.Color == color && friend is null)
                {
                    friend = square;
                    continue;
                }

                // if piece enemy rook or queen mark as pinned and exit
                if (piece.Color != color &&
                    (piece.Type is PieceType.Queen || piece.Type == byWho) &&
                    friend is not null)
                {
                    var type = byWho switch
                    {
                        PieceType.Rook => PinType.Rook,
                        PieceType.Bishop => PinType.Bishop,
                        _ => throw new ArgumentOutOfRangeException(nameof(byWho), byWho, null)
                    };
                    pins.Add(new Pin(friend.Value, (piece, square), type));
                }

                break;
            }
        }

        return pins;
    }

    private bool IsPseudoLegal(Move move, Color color)
    {
        var piece = Board[move.To.Value];
        return piece is null || piece.Color != color;
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

                return line.TakeWhileInclusive(m => Board[m.To.Value] is null);
            });

        return attacked.Select(line => line.Select(m => m.To)).Distinct();
    }

    /// <summary>
    /// Get all attacked squares by piece
    /// </summary>
    private IEnumerable<Square> GetAttackedSquares(Color color)
    {
        var pieces = Board
            .Select((piece, position) => new { piece, position })
            .Where(p => p.piece is not null && p.piece.Color == color);

        var attackedSquares = pieces
            .Select(p => GetAttackedSquares(p.piece!, p.position, color))
            .Flatten()
            .Flatten()
            .Distinct();

        return attackedSquares;
    }
}