using System.Runtime.Serialization;
using Shared;

namespace Habot.Core.Chess;

public readonly record struct Move(Square From, Square To, char? Promotion)
{
    public static Move Serialize(string str)
    {
        var from = Square.Serialize(str.Take(2).CollectString());
        var to = Square.Serialize(str.Skip(2).Take(2).CollectString());

        if (str.Length <= 4)
        {
            return new Move(from, to, null);
        }

        var promotion = char.ToLower(str[4]);
        if (new List<char> { 'q', 'r', 'b', 'n' }.Any(p => p == promotion))
        {
            return new Move(from, to, promotion);
        }

        throw new SerializationException($"""Cannot serialize "{str}" as chess move""");
    }

    public override string ToString() => $"{From}{To}{Promotion}";

    public bool MightBeCastle()
    {
        var castlePairs = new List<(byte, byte)> { (4, 2), (4, 6), (60, 58), (60, 62) };

        var thisClone = this;
        return castlePairs
            .Any(move => thisClone.From.Value == move.Item1 && thisClone.To.Value == move.Item2);
    }
}