using Habot.Engine.Board;
using Habot.Engine.MoveGenerator;
using Habot.Perft;
using Habot.UCI;
using Habot.UCI.Request;

namespace Habot.Engine;

/// <summary>
/// Board UCI adapter
/// </summary>
public class Handler : IUciHandler
{
    private readonly Engine _engine;
    private SmartBoard _playableBoard;

    public Handler()
    {
        _playableBoard = new BoardBuilder<SmartBoard>().Build();
        _engine = new Engine { MoveGenerator = new MoveGenerator.MoveGenerator() };
    }

    public IUciResponse HelloMessage() => IUciResponse.Okay("Hello habot");

    private IUciResponse HandleNewGame()
    {
        _playableBoard = new BoardBuilder<SmartBoard>().Build();
        return IUciResponse.Okay();
    }

    private IUciResponse HandlePosition(IUciPositionRequest request)
    {
        _playableBoard = request switch
        {
            PositionFromFen fromFen => new BoardBuilder<SmartBoard>().SetFen(fromFen.Fen).Build(),
            PositionFromStartPos => new BoardBuilder<SmartBoard>().SetStartingPosition().Build(),
            _ => _playableBoard
        };

        foreach (var move in request.Moves)
        {
            _playableBoard.Move(move);
        }

        return IUciResponse.Okay();
    }

    private IUciResponse HandleGo(Go request)
    {
        // _engine.MoveGenerator = new MoveGenerator.MoveGenerator();
        var move = _engine.Search(request, _playableBoard);
        return IUciResponse.Okay($"bestmove {move}");
    }

    private IUciResponse HandlePerft(UCI.Request.Perft request)
    {
        var perftBoard = new PerftBoard<SmartBoard>(_playableBoard, _playableBoard);
        var moves = perftBoard.Perft(request.Depth).ToList();
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