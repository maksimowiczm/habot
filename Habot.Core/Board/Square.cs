namespace Habot.Core.Board;

public readonly record struct Square(byte Value)
{
    public (byte row, byte column) Position => ((byte row, byte column))(Value / 8, Value % 8);
}