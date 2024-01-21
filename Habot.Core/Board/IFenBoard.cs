using Habot.UCI.Notation;

namespace Habot.Core.Board;

public interface IFenBoard
{
    Fen ToFen();
}