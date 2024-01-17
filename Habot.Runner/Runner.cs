using System.Runtime.Serialization;
using System.Threading.Tasks.Dataflow;
using Habot.UCI;
using Habot.UCI.Request;

namespace Habot.Runner;

public class Runner(IUciHandler engine)
{
    private readonly BufferBlock<IUciRequest> _requestChannel = new();
    private readonly BufferBlock<IUciResponse> _responseChannel = new();

    public async Task Run()
    {
        var runner = new Thread(EngineRun);

        runner.Start();

        Console.WriteLine(engine.HelloMessage().ToString());

        while (true)
        {
            var buffer = Console.ReadLine();
            if (buffer is null)
            {
                continue;
            }

            try
            {
                var request = UciSerializer.SerializeRequest(buffer);
                _requestChannel.Post(request);
                var response = await _responseChannel.ReceiveAsync();
                Console.WriteLine(response);

                if (request is Quit)
                {
                    _requestChannel.Complete();
                    break;
                }
            }
            catch (SerializationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        if (!runner.Join(TimeSpan.FromSeconds(3)))
        {
            await Console.Error.WriteLineAsync("Engine thread killed");
        }
    }

    private async void EngineRun()
    {
        while (true)
        {
            var request = await _requestChannel.ReceiveAsync();
            var response = engine.Handle(request);
            _responseChannel.Post(response);

            if (request is Quit)
            {
                _responseChannel.Complete();
                return;
            }
        }
    }
}