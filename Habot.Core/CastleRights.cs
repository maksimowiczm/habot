using System.Text;
using Habot.UCI.Notation;

namespace Habot.Core;

public class CastleRights : ICloneable
{
    public void Invalidate(Castle castle)
    {
        _rights.Remove(castle);
    }

    public void Invalidate(Color color)
    {
        if (color == Color.White)
        {
            _rights.RemoveAll(c => c is Castle.WhiteKing or Castle.WhiteQueen);
        }
        else if (color == Color.Black)
        {
            _rights.RemoveAll(c => c is Castle.BlackKing or Castle.BlackQueen);
        }
    }

    public bool Has(Castle castle) => _rights.Contains(castle);

    private List<Castle> _rights;

    private CastleRights(string rights)
    {
        if (rights.Contains('-'))
        {
            _rights = new List<Castle>();
            return;
        }

        _rights = rights.Select(ch => ch switch
            {
                'k' => Castle.BlackKing,
                'q' => Castle.BlackQueen,
                'K' => Castle.WhiteKing,
                'Q' => Castle.WhiteQueen,
                _ => throw new ArgumentOutOfRangeException(nameof(ch), ch, null)
            }
        ).ToList();
    }

    public static CastleRights Empty()
    {
        return new CastleRights("");
    }

    public static CastleRights Default()
    {
        return new CastleRights("KQkq");
    }

    public static CastleRights Serialize(string str)
    {
        return new CastleRights(str);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        if (_rights.Contains(Castle.WhiteKing))
        {
            builder.Append('K');
        }

        if (_rights.Contains(Castle.WhiteQueen))
        {
            builder.Append('Q');
        }

        if (_rights.Contains(Castle.BlackKing))
        {
            builder.Append('k');
        }

        if (_rights.Contains(Castle.BlackQueen))
        {
            builder.Append('q');
        }

        return builder.ToString();
    }

    public object Clone() => new CastleRights(ToString());
}