namespace Habot.UCI.Request;

public readonly record struct Perft(int Depth) : IUciRequest;