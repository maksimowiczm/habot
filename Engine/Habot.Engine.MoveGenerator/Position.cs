using Habot.Core;
using Habot.Core.Engine;
using Habot.UCI.Notation;
using Shared;

namespace Habot.Engine.MoveGenerator;

internal partial class Position
{
    private readonly IEnumerable<Piece?> _board;
    private readonly CastleRights _castleRights;
    private readonly Color _colorToMove;
    private readonly Square? _enPassant;

    private readonly Square _kingPosition;
    private readonly IEnumerable<Square> _attackedSquares;
    private readonly IEnumerable<Move> _pseudoMoves;

    public Position(IBoard board)
    {
        _colorToMove = board.ColorToMove;
        _board = board.Board;
        _enPassant = board.EnPassant;
        _castleRights = board.CastleRights;

        _kingPosition = GetKingPosition(_colorToMove);
        _attackedSquares = GetAttackedSquares();
        _pseudoMoves = GetPseudoMoves();
    }

    public IEnumerable<Move> GetLegalMoves()
    {
        // if king is attacked
        if (_attackedSquares.Any(s => s == _kingPosition))
        {
            var kingMoves = GetKingSafeMoves();
            var protectionMoves = GetKingProtectionMoves();
            return kingMoves.Union(protectionMoves);
        }

        var illegalMoves = GetIllegalMoves();
        var legalmoves = _pseudoMoves.Where(m => illegalMoves.All(illegalMove => m != illegalMove));

        return legalmoves;
    }

    private Square GetKingPosition(Color color)
    {
        var kingPosition = _board
            .Select((piece, position) => PieceWithSquare.Create(piece, new Square(position)))
            .WhereNotNull()
            .Single(piece => piece.Type == PieceType.King && piece.Color == color);

        return kingPosition.Position;
    }

    private IEnumerable<Square> GetAttackedSquares()
    {
        var pieces = _board
            .Select((piece, position) => PieceWithSquare.Create(piece, new Square(position)))
            .WhereNotNull()
            .Where(p => p.Color == _colorToMove.Toggle());

        var attackedSquares = pieces
            .Select(GetAttackedSquares)
            .Flatten()
            .Flatten()
            .Distinct();

        return attackedSquares;
    }

    private IEnumerable<IEnumerable<Square>> GetAttackedSquares(PieceWithSquare piece)
    {
        if (piece.Type is PieceType.Pawn)
        {
            return [GetAttackedByPawn(piece.Position.Value, piece.Color)];
        }

        var moves = piece.GetStupidMoves();
        var attacked = moves.Select(line => line.TakeWhileInclusive(m => _board.ElementAt(m.To.Value) is null));

        return attacked.Select(line => line.Select(m => m.To)).Distinct();
    }

    private static IEnumerable<Square> GetAttackedByPawn(int position, Color color)
    {
        var attacked = PieceExtensions
            .GetStupidPawnMoves(color, new Square(position))
            .Last();

        return attacked.Select(m => m.To);
    }

    private IEnumerable<Move> GetPseudoMoves()
    {
        var pieces = _board
            .Select((piece, position) => PieceWithSquare.Create(piece, new Square(position)))
            .WhereNotNull()
            .Where(p => p.Color == _colorToMove);

        var moves = pieces.Select(GetPseudoMoves).Flatten();

        return moves.Union(GetCastles());
    }

    private IEnumerable<Move> GetPseudoMoves(PieceWithSquare piece)
    {
        var moves = piece.GetStupidMoves();

        if (piece.Type == PieceType.Pawn)
        {
            var forward = moves.First().TakeWhile(m => _board.ElementAt(m.To.Value) is null);
            // todo Validate enpassants
            // var enPassant = moves.Last().Where(m => m.To == _enPassant && ValidMove(m));
            var enPassant = Enumerable.Empty<Move>();

            var capture = moves
                .Last()
                .Where(m =>
                    _board.ElementAt(m.To.Value) is not null &&
                    _board.ElementAt(m.To.Value)!.Color != piece.Color
                );

            return forward.Union(enPassant).Union(capture);
        }

        var legal = moves
            .Select(line =>
            {
                var scanner = line.TakeWhile(m => IsPseudoLegal(m));

                // stop at capture
                if (scanner.Any(m =>
                    _board.ElementAt(m.To.Value) is not null &&
                    _board.ElementAt(m.To.Value)!.Color != _colorToMove)
                )
                {
                    return scanner.TakeWhileInclusive(m => _board.ElementAt(m.To.Value) is null);
                }

                return scanner;
            })
            .Flatten();

        return legal;
    }

    private bool IsPseudoLegal(Move move)
    {
        var piece = _board.ElementAt(move.To.Value);
        return piece is null || piece.Color != _colorToMove;
    }

    private IEnumerable<Move> GetIllegalMoves()
    {
        var illegalLKingMoves = _pseudoMoves
            .Where(m => m.From == _kingPosition && _attackedSquares.All(a => a != m.To));

        var pins = GetIllegalPinnedPiecesMoves();

        return illegalLKingMoves.Union(pins);
    }

    /// <summary>
    /// Represents piece pin.
    /// </summary>
    /// <param name="Pinned">Pinned piece.</param>
    /// <param name="By">Piece that pins.</param>
    private readonly record struct Pin(Square Pinned, (Piece, Square) By);

    private IEnumerable<Move> GetIllegalPinnedPiecesMoves()
    {
        // Remove illegal moves with pinned pieces
        var pinnedPieces = GetPins();

        // Enumerable of illegal moves by pinned pieces
        var illegalMoves = _pseudoMoves
            .Join(pinnedPieces,
                move => move.From,
                pin => pin.Pinned,
                (move, pin) => (move, pin)
            )
            .Where(tuple =>
            {
                var (move, pin) = tuple;
                var (_, pinner) = pin;
                var (pinnerPiece, pinnerPosition) = pinner;

                // 😂💀
                var line = pinnerPiece
                    .GetStupidMoves(pinnerPosition)
                    .Where(l => l.Any(m => m.To == _kingPosition))
                    .Flatten()
                    .Select(m => m.To)
                    .SkipLast(1)
                    .Append(pinnerPosition);

                return line.All(s => s != move.To);
            })
            .Select(tuple => tuple.move);

        return illegalMoves;
    }

    private IEnumerable<Pin> GetPins()
    {
        var rookPins = GetPins(PieceType.Rook);
        var bishopPins = GetPins(PieceType.Bishop);

        return rookPins.Union(bishopPins);
    }

    private List<Pin> GetPins(PieceType byWho)
    {
        var possiblePins = new Piece(byWho, _colorToMove)
            .GetStupidMoves(_kingPosition)
            .Where(line => line.Any())
            .Select(line => line.Select(m => m.To));

        // black magic xd
        var pins = new List<Pin>();
        foreach (var line in possiblePins)
        {
            Square? friend = null;

            foreach (var square in line)
            {
                var piece = _board.ElementAt(square.Value);

                // skip empty square
                if (piece is null)
                {
                    continue;
                }

                // if piece is first friendly piece mark as friend
                if (piece.Color == _colorToMove && friend is null)
                {
                    friend = square;
                    continue;
                }

                // if piece enemy rook or queen mark as pinned and exit
                if (piece.Color != _colorToMove &&
                    (piece.Type is PieceType.Queen || piece.Type == byWho) &&
                    friend is not null)
                {
                    pins.Add(new Pin(friend.Value, (piece, square)));
                }

                break;
            }
        }

        return pins;
    }

    /// <summary>
    /// Gets legal castles. Does not check if king is on check.
    /// </summary>
    private IEnumerable<Move> GetCastles()
    {
        // [castle] = ([hasToBeSafe], [hasToBeEmpty])
        var squareThatHaveToBeSafeToCastle =
            new Dictionary<Castle, (IEnumerable<int> HasToBeSafe, IEnumerable<int> HasToBeEmpty)>
            {
                [Castle.WhiteKing] = ([5, 6], [5, 6]),
                [Castle.WhiteQueen] = ([2, 3], [1, 2, 3]),
                [Castle.BlackKing] = ([61, 62], [61, 62]),
                [Castle.BlackQueen] = ([58, 59], [57, 58, 59])
            };

        var colorCastles = _colorToMove.Castles();

        var possibleCastles = squareThatHaveToBeSafeToCastle
            .Where(p =>
                // match castle colors
                colorCastles.Any(c => c == p.Key) &&
                // can do this castle
                _castleRights.Has(p.Key) &&
                // none of the squares is attacked
                p.Value.HasToBeSafe.All(v => _attackedSquares.All(s => s != new Square(v))) &&
                // none piece between king and rook
                p.Value.HasToBeEmpty.All(v => _board.ElementAt(v) is null)
            )
            .Select(p => p.Key);

        return possibleCastles.Select(c => c.ToMove());
    }

    /// <summary>
    /// Get legal king moves after check.
    /// </summary>
    private IEnumerable<Move> GetKingSafeMoves()
    {
        var enemies = _board
            .Select((piece, position) => PieceWithSquare.Create(piece, new Square(position)))
            .WhereNotNull()
            .Where(p => p.Color == _colorToMove.Toggle());

        var unsafeSquares = enemies
            .Select(GetUnsafeSquares)
            .WhereNotNull()
            .Flatten();

        var illegalMoves = _pseudoMoves
            .Where(m => m.From == _kingPosition &&
                        unsafeSquares.All(s => s != m.To) &&
                        _attackedSquares.All(s => s != m.To)
            );

        return illegalMoves;
    }

    private List<Square>? GetUnsafeSquares(PieceWithSquare piece)
    {
        // consider only sliding pieces
        if (piece.Type is not (PieceType.Queen or PieceType.Rook or PieceType.Bishop))
        {
            return null;
        }

        var lines = piece.GetStupidMoves();

        // get line with king
        var line = lines
            .Select(l => l.Select(m => m.To))
            .FirstOrDefault(l => l.Any(s => s == _kingPosition));

        if (line is null)
        {
            return null;
        }

        var attackedLine = line;

        var attackedSquares = new List<Square>();
        foreach (var square in attackedLine)
        {
            var pieceOnSquare = _board.ElementAt(square.Value);

            // skip empty squares
            if (pieceOnSquare is null)
            {
                attackedSquares.Add(square);
                continue;
            }

            // if there is piece on attacked square it means that king is hidden behind it so it is not attacked
            if (square != _kingPosition)
            {
                return null;
            }

            // if king is on this square it means that it can not move on this line at all
            attackedSquares = attackedLine.ToList();
            break;
        }

        return attackedSquares;
    }


    /// <summary>
    /// Get moves that can protect king after check.
    /// </summary>
    private List<Move> GetKingProtectionMoves()
    {
        var color = _colorToMove.Toggle();

        var enemies = _board
            .Select((piece, position) => PieceWithSquare.Create(piece, new Square(position)))
            .WhereNotNull()
            .Where(p => p.Color == color);

        var attackedSquares = enemies
            .Select<PieceWithSquare, (PieceWithSquare, List<Square>)?>(piece =>
            {
                var attackedSquares = GetAttackedSquares(piece);
                var line = attackedSquares.FirstOrDefault(line => line.Any(s => s == _kingPosition));
                if (line is null)
                {
                    return null;
                }

                if (piece.Type == PieceType.Pawn)
                {
                    return (piece, new List<Square> { piece.Position });
                }

                if (piece.Type == PieceType.Knight)
                {
                    if (attackedSquares.First().Any(s => s == _kingPosition))
                    {
                        return (piece, new List<Square> { piece.Position, _kingPosition });
                    }

                    return null;
                }


                var listLine = line.ToList();
                listLine.Add(piece.Position);
                return (piece, listLine);
            })
            .WhereNotNull();

        var pinnedPieces = GetPins();

        var legalMoves = _pseudoMoves
            .Where(m =>
                m.From != _kingPosition &&
                attackedSquares
                    .Select(x => x!.Value.Item2)
                    .IntersectMultiple()
                    .Where(s => s != _kingPosition)
                    .Any(s => s == m.To) &&
                pinnedPieces.All(pin => pin.Pinned != m.From)
            )
            .ToList();

        if (_enPassant is null)
        {
            return legalMoves;
        }

        // add en passant if exists
        var attackerPawn = attackedSquares.SingleOrDefault(x => x!.Value.Item1.Type == PieceType.Pawn);
        if (attackerPawn is null)
        {
            return legalMoves;
        }

        var pawnMoves = _pseudoMoves.Where(m => _board.ElementAt(m.From.Value)?.Type is PieceType.Pawn);
        var enPassants = pawnMoves.Where(m => m.To == _enPassant);
        legalMoves.AddRange(enPassants);

        return legalMoves;
    }
}