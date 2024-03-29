using System.Runtime.Serialization;
using Habot.UCI.Notation;
using Shared;

namespace Habot.UCI.Request;

public readonly record struct Position(
    IEnumerable<Move> Moves
) : IUciPositionRequest
{
    public static IUciPositionRequest Serialize(string str)
    {
        var tokens = str.Trim().Split(null);

        var moves = tokens.SkipWhile(t => t != "moves").Skip(1).Select(Move.Serialize).ToList();

        return tokens switch
        {
            ["position", "moves", ..] => new Position(moves),
            ["position", "fen", ..] => new PositionFromFen(new Fen(string.Join(" ", tokens.Skip(2).Take(6))), moves),
            ["position", "startpos", ..] => new PositionFromStartPos(moves),
            _ => throw new SerializationException(
                $"""Cannot serialize "{tokens.CollectString()}" as UCI position command""")
        };
    }
}