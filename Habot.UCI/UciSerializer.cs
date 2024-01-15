using System.Runtime.Serialization;
using Habot.UCI.Request;

namespace Habot.UCI;

public static class UciSerializer
{
    public static IUciRequest SerializeRequest(string str)
    {
        var tokens = str.Trim().Split(null);

        return tokens switch
        {
            ["uci"] => new Uci(),
            ["isready"] => new IsReady(),
            ["ucinewgame"] => new UciNewGame(),
            ["stop"] => new Stop(),
            ["quit"] => new Quit(),
            ["setoption", "name", var name, "value", var value] => new SetOption(name, value),
            ["position", ..] => Position.Serialize(str),
            ["go", ..] => Go.Serialize(str),
            _ => throw new SerializationException($"""Unknown command "{str}".""")
        };
    }
}