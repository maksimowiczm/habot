using System.Runtime.Serialization;

namespace Habot.Core.Chess;

public readonly record struct Square
{
    public byte Value { get; }
    public (byte row, byte column) Position => ((byte row, byte column))(Value / 8, Value % 8);

    public Square(byte value)
    {
        Value = value;
    }

    public Square(int value)
    {
        Value = (byte)(value);
    }

    public Square(byte row, byte column)
    {
        Value = (byte)(row * 8 + column);
    }

    public Square(int row, int column)
    {
        Value = (byte)(row * 8 + column);
    }

    private static readonly Dictionary<char, byte> FilesDictionary = new()
        { ['a'] = 0, ['b'] = 1, ['c'] = 2, ['d'] = 3, ['e'] = 4, ['f'] = 5, ['g'] = 6, ['h'] = 7 };


    public static Square Serialize(string str)
    {
        if (str.Length != 2)
        {
            throw new SerializationException($"""Cannot serialize "{str}" as chess square.""");
        }


        var letter = char.ToLower(str[0]);

        if (!FilesDictionary.TryGetValue(letter, out var col))
        {
            throw new SerializationException($"""Cannot serialize "{str}" as chess square.""");
        }

        var digit = str[1] - '0' - 1;
        if (digit is < 0 or > 7)
        {
            throw new SerializationException($"""Cannot serialize "{str}" as chess square.""");
        }

        return new Square((byte)digit, col);
    }

    private static readonly Dictionary<byte, char> ReversedFilesDictionary =
        FilesDictionary.ToDictionary(x => x.Value, x => x.Key);

    public override string ToString() => $"{ReversedFilesDictionary[Position.column]}{Position.row + 1}";
}