namespace Habot.UCI.Notation;

/// <summary>
/// Represents FEN notation.
/// https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation
/// </summary>
public readonly record struct Fen(string Value)
{
    public override string ToString() => Value;
}