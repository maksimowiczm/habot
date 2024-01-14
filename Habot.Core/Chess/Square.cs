namespace Habot.Core.Chess;

public readonly record struct Square
{
    public byte Value { get; }
    public (byte row, byte column) Position => ((byte row, byte column))(Value / 8, Value % 8);

    public Square(byte value)
    {
        Value = value;
    }

    public Square(byte row, byte column)
    {
        Value = (byte)(row * 8 + column);
    }
}