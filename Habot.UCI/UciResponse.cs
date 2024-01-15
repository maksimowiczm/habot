namespace Habot.UCI;

public readonly record struct UciResponse(string Value)
{
    public override string ToString() => Value;
};