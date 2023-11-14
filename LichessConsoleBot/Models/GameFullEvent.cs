namespace LichessConsoleBot.Models;

public class GameFullEvent
{
    public string id { get; set; }
    public bool rated { get; set; }
    public string type { get; set; }
    public Variant variant { get; set; }
    public Clock clock { get; set; }
    public string speed { get; set; }
    public Perf perf { get; set; }
    public long createdAt { get; set; }
    public PlayerObj white { get; set; }
    public PlayerObj black { get; set; }
    public string initialFen { get; set; }
    public GameStateEvent state { get; set; }
}


public class Clock
{
    public int initial { get; set; }
    public int increment { get; set; }
}

public class PlayerObj
{
    public string id { get; set; }
    public string name { get; set; }
    public bool provisional { get; set; }
    public int rating { get; set; }
    public string title { get; set; }
}
