namespace Habot.UCI.Notation;

public enum Castle
{
    BlackKing,
    BlackQueen,
    WhiteKing,
    WhiteQueen
}

public static class CastleExtensions
{
    public static Move ToMove(this Castle castle) =>
        castle switch
        {
            Castle.BlackKing => new Move(new Square(60), new Square(62)),
            Castle.BlackQueen => new Move(new Square(60), new Square(58)),
            Castle.WhiteKing => new Move(new Square(4), new Square(6)),
            Castle.WhiteQueen => new Move(new Square(4), new Square(2)),
            _ => throw new ArgumentOutOfRangeException(nameof(castle), castle, null)
        };
}