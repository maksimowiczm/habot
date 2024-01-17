namespace Habot.UCI;

public interface IUciResponse
{
    public static IUciResponse Okay(string response) => new UciResponseString(response);
    public static IUciResponse Okay() => new UciResponse();
    public static IUciResponse UnknownCommand() => new UciResponseUnknownCommand();
}

internal readonly record struct UciResponse : IUciResponse
{
    public override string ToString() => "";
}

internal readonly record struct UciResponseString(string Value) : IUciResponse
{
    public override string ToString() => Value;
}

internal record UciResponseUnknownCommand : IUciResponse
{
    public override string ToString() => "Unknown command";
}