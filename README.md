### Chess engine

Actually, this engine is depth 1 with the most monstrous class for move
generation I have ever written (yet).

It passes all [perft test](./Perft/Habot.Perft.Tests/PerftTestsData.cs) (with
small depth, but it seems to be complete).

The Engine runner uses 2 parallel threads, one for user input and the other one
for engine evaluation (depth 1 btw 💀).

UCI command are parsed with custom
[UCI Serializer](./Habot.UCI/UciSerializer.cs). Upon successful parsing,
[the runner](./Habot.Runner/Runner.cs) sends request to
[the handler](./Habot.UCI/IUciHandler.cs)
([implementation](./Engine/Habot.Engine/Handler.cs)).

Moves are generated using Mailbox board representation.
[move generator](./Engine/Habot.Engine.MoveGenerator/SmartBoard.cs)

Board is evaluated with piece-type score.
[evaluator](./Engine/Habot.Engine/Engine.cs)

Board functionality is split across interfaces (I love SOLID). Whole abstraction
is stored in [Habot.Core](./Habot.Core/). The implementation seem to be some
kind of inheritance hell, but at least it is single responsibility principle :).
