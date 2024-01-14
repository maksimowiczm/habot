namespace Habot.Core.Chess;

public readonly record struct Move(Square From, Square To)
{
    public static Move Serialize(string str)
    {
        throw new NotImplementedException();
    }
}