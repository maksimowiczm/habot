namespace Habot.UCI;

public interface IUciHandler
{
    IUciResponse HelloMessage();
    IUciResponse Handle(IUciRequest request);
}