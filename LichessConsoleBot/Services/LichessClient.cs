using LichessConsoleBot.Models;
using System.Text.Json;

namespace LichessConsoleBot.Services;

public delegate void GameStarted(Game game);
public delegate void GameFinished(string gameId);
public delegate void GameStateChanged(string gameId, GameStateEvent gameState);

public class LichessClient
{
    private readonly HttpClient _httpClient;
    private List<string> _gamesStarted = new List<string>();
    public event GameStarted? GameStarted;
    public event GameFinished? GameFinished;
    public event GameStateChanged? GameStateChanged;

    public LichessClient(HttpClient httpClient, string lichessBotToken)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://lichess.org");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {lichessBotToken}");
    }

    public async Task AcceptChallenge(string challengeId)
    {
        await _httpClient.PostAsync($"https://lichess.org/api/challenge/{challengeId}/accept", null);
    }

    public async Task StreamEvents()
    {
        using (HttpResponseMessage response = await _httpClient.GetAsync("https://lichess.org/api/stream/event", HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();

            try
            {
                await foreach (string stringifedEvent in response.Content!.ReadStringFromNdjsonAsync())
                {
                    if (string.IsNullOrEmpty(stringifedEvent))
                    {
                        Console.WriteLine($"No events received. Continuing.");
                        await Task.Delay(3000);
                        continue;
                    }
                    var ev = JsonSerializer.Deserialize<LichessEvent>(stringifedEvent);
                    if (ev.type == EventTypes.Challenge && ev.challenge.status == "created")
                    {
                        Console.WriteLine($"Challenge received from {ev.challenge.challenger.name}, id {ev.challenge.id}");
                        await AcceptChallenge(ev.challenge.id);
                    }
                    if (ev.type == EventTypes.GameStart && !_gamesStarted.Any(id => id == ev.game.id))
                    {
                        _gamesStarted.Add(ev.game.id);
                        Console.WriteLine($"Game started with {ev.game.opponent.username}: {ev.game.opponent.rating}, Game id: {ev.game.id}. My color? {ev.game.color}");

                        GameStarted?.Invoke(ev.game);
                    }
                    if (ev.type == EventTypes.GameFinish && _gamesStarted.Any(id => id == ev.game.id))
                    {
                        _gamesStarted.Add(ev.game.id);
                        Console.WriteLine($"Game Finished with {ev.game.opponent.username}: {ev.game.opponent.rating}, Game id: {ev.game.id}. Winner {ev.game.winner}");

                        GameFinished?.Invoke(ev.game.id);
                    }
                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error streaming events. {ex.Message}");
            }
        }
        Console.WriteLine("Finished streaming events");
    }

    public async Task StreamGameEvents(string gameId)
    {
        try
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync($"api/bot/game/stream/{gameId}", HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                await foreach (string eventStringified in response.Content!.ReadStringFromNdjsonAsync())
                {
                    if (string.IsNullOrEmpty(eventStringified))
                    {
                        Console.WriteLine($"No events received for game {gameId}. Continuing.");
                        await Task.Delay(1000);
                        continue;
                    }

                    if (eventStringified.Contains(EventTypes.GameFull))
                    {
                        var eventGame = JsonSerializer.Deserialize<GameFullEvent>(eventStringified);
                        Console.WriteLine($"GameFull event: {eventGame.id} has status {eventGame.state.status} and moves {eventGame.state.moves}.");
                        GameStateChanged?.Invoke(gameId, eventGame.state);
                    }
                    else if (eventStringified.Contains(EventTypes.GameState))
                    {
                        var eventGameState = JsonSerializer.Deserialize<GameStateEvent>(eventStringified);
                        Console.WriteLine($"GameState event processed {gameId}: {eventGameState.moves}");
                        GameStateChanged?.Invoke(gameId, eventGameState);
                    }
                    else
                    {
                        Console.WriteLine($"Event ignored {eventStringified}");
                    }
                    await Task.Delay(1000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception when streaming game events {gameId}. {ex.Message}");
        }

    }

    public async Task TryMakeMove(string gameId, string move)
    {
        var res = await _httpClient.PostAsync($"api/bot/game/{gameId}/move/{move}", null);
        var rescontent = await res.Content.ReadAsStringAsync();
        Console.WriteLine($"Move {move} in game {gameId} resulted in {res.StatusCode} - {rescontent}");
    }
}
