// See https://aka.ms/new-console-template for more information
using LichessConsoleBot.Models;
using LichessConsoleBot.Services;
using Microsoft.Extensions.Configuration;


Console.WriteLine("I am a ChessBot and I want to play people on Lichess!");

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

var config = configuration.Build();
var lichessToken = config["LichessToken"];

using var httpClient = new HttpClient();

var runningGames = new Dictionary<string, (Game game, CancellationTokenSource cancellationToken, GameState gameState, ChessAi ai)>();
var lichessClient = new LichessClient(httpClient, lichessToken!);
lichessClient.GameStarted += HandleGameStartedAsync;
lichessClient.GameFinished += HandleGameFinished;
lichessClient.GameStateChanged += HandleGameStateChanged;

Task.Run(() => lichessClient.StreamEvents());

while (true)
{
    Console.WriteLine("Press any key to kill the bot. \n\n");

    var move = Console.ReadLine();

    break;
}

void HandleGameStartedAsync(Game game)
{
    Console.WriteLine("Game started! " + game.id);
    var cancellationTokenSource = new CancellationTokenSource();
    var token = cancellationTokenSource.Token;
    var chessAi = new ChessAi();
    runningGames![game.id] = (game, cancellationTokenSource, new GameState(), chessAi);
    chessAi.MoveFound += (string move) => HandleChessAiMoveFound(move, game.id);

    if (game.color == "white")
    {
        chessAi!.FindMoveForPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    Task.Run(() => lichessClient!.StreamGameEvents(game.id), token);
#pragma warning restore CS4014
}

void HandleChessAiMoveFound(string move, string gameId)
{
    Console.WriteLine($"Trying to play {move} in {gameId}");
    lichessClient.TryMakeMove(gameId, move);
}

void HandleGameFinished(string gameId)
{
    Console.WriteLine("Game finished! " + gameId);
    runningGames[gameId].cancellationToken.Cancel();
    runningGames.Remove(gameId);
}

void HandleGameStateChanged(string gameId, GameStateEvent gameStateEvent)
{
    Console.WriteLine($"Handling game state changed {gameId} - {gameStateEvent.moves}");
    var gameState = runningGames[gameId].gameState;
    var (isMoveAccepted, whosTurnIsIt, fen) = gameState.ChangeMoves(gameStateEvent.moves);
    Console.WriteLine($"Moves changed {isMoveAccepted} - {whosTurnIsIt.ToString()} to play. {fen}");
    if (isMoveAccepted && gameState.GetWhosTurnIsIt().ToString().ToLower() == runningGames[gameId].game.color)
    {
        Console.WriteLine($"Getting moves from AI.");
        Task.Run(() => runningGames[gameId]!.ai.FindMoveForPosition(fen));
    }
}

