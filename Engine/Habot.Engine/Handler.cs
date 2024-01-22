using Habot.Engine.Board;
using Habot.UCI;
using Habot.UCI.Request;

namespace Habot.Engine;

/// <summary>
/// Board UCI adapter
/// </summary>
public class Handler : IUciHandler
{
    private Board.Engine _board = new BoardBuilder<Board.Engine>().Build();

    public IUciResponse HelloMessage() => IUciResponse.Okay("Hello habot");

    private IUciResponse HandleNewGame()
    {
        _board = new BoardBuilder<Board.Engine>().Build();
        return IUciResponse.Okay();
    }

    private IUciResponse HandlePosition(IUciPositionRequest request)
    {
        _board = request switch
        {
            PositionFromFen fromFen => new BoardBuilder<Board.Engine>().SetFen(fromFen.Fen).Build(),
            PositionFromStartPos => new BoardBuilder<Board.Engine>().SetStartingPosition().Build(),
            _ => _board
        };

        foreach (var move in request.Moves)
        {
            _board.Move(move);
        }

        return IUciResponse.Okay();
    }

    private IUciResponse HandleGo(Go request)
    {
        var move = _board.Search(request);
        return IUciResponse.Okay($"bestmove {move}");
    }

    private IUciResponse HandlePerft(UCI.Request.Perft request)
    {
        var moves = _board.Perft(request.Depth).ToList();
        var sum = moves.Select(m => m.Count).Sum();
        var movesStrings = moves.Select(m => m.ToString()).OrderBy(s => s);
        var output = string.Join("\n", movesStrings) + $"\n\n{sum}";
        return IUciResponse.Okay(output);
    }

    public IUciResponse Handle(IUciRequest request) =>
        request switch
        {
            Uci => IUciResponse.Okay("id name Habot random\nid author hakoski\nuciok"),
            IsReady => IUciResponse.Okay("readyok"),
            UciNewGame => HandleNewGame(),
            Quit => IUciResponse.Okay(),
            Stop => IUciResponse.Okay(),
            IUciPositionRequest uciRequestWithMoves => HandlePosition(uciRequestWithMoves),
            Go go => HandleGo(go),
            UCI.Request.Perft perft => HandlePerft(perft),
            _ => IUciResponse.UnknownCommand()
        };
}