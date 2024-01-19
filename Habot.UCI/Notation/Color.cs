namespace Habot.UCI.Notation;

public enum Color
{
    White,
    Black
}

public static class ColorExtensions
{
    public static Color Toggle(this Color color) =>
        color switch
        {
            Color.White => Color.Black,
            Color.Black => Color.White,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
}