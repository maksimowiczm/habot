namespace Habot.UCI;

public interface IUciHandler
{
    UciResponse HelloMessage();
    UciResponse Handle(IUciRequest request);
}