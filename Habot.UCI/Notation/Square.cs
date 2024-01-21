using System.Runtime.Serialization;

namespace Habot.UCI.Notation;

public readonly record struct Square
{
    public int Value { get; }
    public (int row, int column) Position => (Value / 8, Value % 8);

    public Square(int value)
    {
        Value = value;
    }

    public Square(int row, int column)
    {
        Value = row * 8 + column;
    }

    public Square((int row, int column) value)
    {
        var (row, column) = value;
        Value = row * 8 + column;
    }

    private static readonly Dictionary<char, int> FilesDictionary = new()
        { ['a'] = 0, ['b'] = 1, ['c'] = 2, ['d'] = 3, ['e'] = 4, ['f'] = 5, ['g'] = 6, ['h'] = 7 };

    public static Square Serialize(string str)
    {
        if (str.Length != 2)
        {
            throw new SerializationException($"""Cannot serialize "{str}" as chess square.""");
        }


        var letter = char.ToLower(str[0]);

        if (!FilesDictionary.TryGetValue(letter, out var col))
        {
            throw new SerializationException($"""Cannot serialize "{str}" as chess square.""");
        }

        var digit = str[1] - '0' - 1;
        if (digit is < 0 or > 7)
        {
            throw new SerializationException($"""Cannot serialize "{str}" as chess square.""");
        }

        return new Square(digit, col);
    }

    private static readonly Dictionary<int, char> ReversedFilesDictionary =
        FilesDictionary.ToDictionary(x => x.Value, x => x.Key);

    public override string ToString() => $"{ReversedFilesDictionary[Position.column]}{Position.row + 1}";


    public IEnumerable<IEnumerable<Square>> GetRookLines()
    {
        var (row, column) = Position;
        yield return GetRookLineColumn();
        yield return GetRookLineRow();
        yield break;

        IEnumerable<Square> GetRookLineColumn() => Enumerable.Range(0, 8).Select(i => new Square(i, column));
        IEnumerable<Square> GetRookLineRow() => Enumerable.Range(0, 8).Select(i => new Square(row, i));
    }

    // 💀
    public IEnumerable<IEnumerable<Square>> GetDiagonals()
    {
        {
            var startLeftRight = Position;
            while (startLeftRight.row - 1 >= 0 && startLeftRight.column - 1 >= 0)
            {
                startLeftRight.row -= 1;
                startLeftRight.column -= 1;
            }

            var leftRight = new List<Square>();
            while (startLeftRight is { row: < 8, column: < 8 })
            {
                leftRight.Add(new Square(startLeftRight.row, startLeftRight.column));
                startLeftRight.row += 1;
                startLeftRight.column += 1;
            }

            yield return leftRight;
        }

        var startRightLeft = Position;
        while (startRightLeft.row + 1 < 8 && startRightLeft.column - 1 >= 0)
        {
            startRightLeft.row += 1;
            startRightLeft.column -= 1;
        }

        var rightLeft = new List<Square>();
        while (startRightLeft is { row: < 8, column: < 8 })
        {
            rightLeft.Add(new Square(startRightLeft.row, startRightLeft.column));
            startRightLeft.row -= 1;
            startRightLeft.column += 1;
        }

        yield return rightLeft;
    }

    public bool IsSameDiagonal(Square other)
    {
        {
            var startLeftRight = Position;
            while (startLeftRight.row - 1 >= 0 && startLeftRight.column - 1 >= 0)
            {
                startLeftRight.row -= 1;
                startLeftRight.column -= 1;
            }

            while (startLeftRight is { row: < 8, column: < 8 })
            {
                if (other.Position == startLeftRight)
                {
                    return true;
                }

                startLeftRight.row += 1;
                startLeftRight.column += 1;
            }
        }

        var startRightLeft = Position;
        while (startRightLeft.row + 1 < 8 && startRightLeft.column - 1 >= 0)
        {
            startRightLeft.row += 1;
            startRightLeft.column -= 1;
        }

        while (startRightLeft is { row: > 0, column: < 8 })
        {
            if (other.Position == startRightLeft)
            {
                return true;
            }

            startRightLeft.row -= 1;
            startRightLeft.column += 1;
        }

        return false;
    }
}

public static class SquareExtensions
{
    public static string EnPassantToFen(this Square? square) => square switch
    {
        null => "-",
        _ => square!.ToString()!
    };
}