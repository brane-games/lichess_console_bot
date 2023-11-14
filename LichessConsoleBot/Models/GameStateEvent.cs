namespace LichessConsoleBot.Models;

public class GameStateEvent : IEvent
{
    public string type { get; set; }
    public string moves { get; set; }
    public int wtime { get; set; }
    public int btime { get; set; }
    public int winc { get; set; }
    public int binc { get; set; }
    public string status { get; set; }

}
