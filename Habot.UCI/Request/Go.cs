using Habot.UCI.Notation;

namespace Habot.UCI.Request;

public readonly record struct Go(
    IEnumerable<Move> Moves,
    bool Ponder,
    uint? WhiteTime,
    uint? BlackTime,
    uint? WhiteIncrement,
    uint? BlackIncrement,
    uint? MovesToGo,
    uint? Depth,
    uint? Nodes,
    uint? Mate,
    uint? MoveTime,
    bool Infinite
) : IUciRequest
{
    private static T? MapKeyValue<T>(IEnumerable<string> tokens, string key, IFormatProvider? provider = null)
        where T : struct, IParsable<T>
    {
        var str = tokens.SkipWhile(t => t != key).Skip(1).Take(1).ToList();
        if (str.Count != 1)
        {
            return null;
        }

        return T.Parse(str[0], provider);
    }

    public static Go Serialize(string str)
    {
        var tokens = str.Trim().Split(null);

        var moves = tokens
            .SkipWhile(t => t != "searchmoves")
            .Skip(1)
            .Where(t => t.Length == 4)
            .TakeWhile(t => t != "winc" && t != "binc" && t != "mate")
            .Select(Move.Serialize)
            .ToList();

        var whiteTime = MapKeyValue<uint>(tokens, "wtime");
        var blackTime = MapKeyValue<uint>(tokens, "btime");
        var whiteIncrement = MapKeyValue<uint>(tokens, "winc");
        var blackIncrement = MapKeyValue<uint>(tokens, "binc");
        var movesToGo = MapKeyValue<uint>(tokens, "movestogo");
        var depth = MapKeyValue<uint>(tokens, "depth");
        var nodes = MapKeyValue<uint>(tokens, "nodes");
        var mate = MapKeyValue<uint>(tokens, "mate");
        var moveTime = MapKeyValue<uint>(tokens, "movetime");
        var infinite = tokens.Any(t => t == "infinite") || tokens.Length == 1;
        var ponder = tokens.Any(t => t == "ponder");

        return new Go(moves, ponder, whiteTime, blackTime, whiteIncrement, blackIncrement, movesToGo, depth, nodes,
            mate, moveTime, infinite);
    }
};