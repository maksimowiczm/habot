using Habot.Core.Chess;

namespace Habot.Core.Mailbox;

public interface IMailboxBoard
{
    public Dictionary<uint, Piece> StartingPositionPiecesMap()
    {
        var dictionary = new Dictionary<uint, Piece>()
        {
            [0u] = new(Piece.PieceType.Rook, Color.White),
            [1u] = new(Piece.PieceType.Knight, Color.White),
            [2u] = new(Piece.PieceType.Bishop, Color.White),
            [3u] = new(Piece.PieceType.Queen, Color.White),
            [4u] = new(Piece.PieceType.King, Color.White),
            [5u] = new(Piece.PieceType.Bishop, Color.White),
            [6u] = new(Piece.PieceType.Knight, Color.White),
            [7u] = new(Piece.PieceType.Rook, Color.White),

            [63u] = new(Piece.PieceType.Rook, Color.Black),
            [62u] = new(Piece.PieceType.Knight, Color.Black),
            [61u] = new(Piece.PieceType.Bishop, Color.Black),
            [60u] = new(Piece.PieceType.King, Color.Black),
            [59u] = new(Piece.PieceType.Queen, Color.Black),
            [58u] = new(Piece.PieceType.Bishop, Color.Black),
            [57u] = new(Piece.PieceType.Knight, Color.Black),
            [56u] = new(Piece.PieceType.Rook, Color.Black),
        };

        foreach (var i in Enumerable.Range(8, 16))
        {
            dictionary.Add((uint)i, new Piece(Piece.PieceType.Pawn, Color.White));
        }

        foreach (var i in Enumerable.Range(48, 56))
        {
            dictionary.Add((uint)i, new Piece(Piece.PieceType.Pawn, Color.Black));
        }

        return dictionary;
    }
}