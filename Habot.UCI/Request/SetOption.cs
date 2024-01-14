namespace Habot.UCI.Request;

public readonly record struct SetOption(
    string Name,
    string Value
) : IUciRequest;