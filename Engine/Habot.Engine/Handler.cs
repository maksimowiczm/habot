using Habot.UCI;
using Habot.UCI.Request;

namespace Habot.Engine;

/// <summary>
/// Board UCI adapter
/// </summary>
public class Handler : IUciHandler
{
    private Board.Engine _board = Board.Engine.Create();

    public IUciResponse HelloMessage() => IUciResponse.Okay("Hello habot");

    private IUciResponse HandleNewGame()
    {
        _board = Board.Engine.Create();
        return IUciResponse.Okay();
    }

    private IUciResponse HandlePosition(IUciPositionRequest request)
    {
        _board = request switch
        {
            PositionFromFen fromFen => Board.Engine.Create(fromFen.Fen),
            PositionFromStartPos => Board.Engine.Create(),
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
            _ => IUciResponse.UnknownCommand()
        };
}