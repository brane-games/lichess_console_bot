namespace LichessConsoleBot.Models;
public class LichessEvent : IEvent
{
    public Game game { get; set; }
    public Challenge challenge { get; set; }
    public string type { get ; set ; }
}

public class Game
{
    public string gameId { get; set; }
    public string fullId { get; set; }
    public string color { get; set; }
    public string fen { get; set; }
    public bool hasMoved { get; set; }
    public bool isMyTurn { get; set; }
    public string lastMove { get; set; }
    public Opponent opponent { get; set; }
    public string perf { get; set; }
    public bool rated { get; set; }
    public int secondsLeft { get; set; }
    public string source { get; set; }
    public Status status { get; set; }
    public string speed { get; set; }
    public Variant variant { get; set; }
    public Compat compat { get; set; }
    public string winner { get; set; }
    public int ratingDiff { get; set; }
    public string id { get; set; }
}


public class Opponent
{
    public string id { get; set; }
    public string username { get; set; }
    public int rating { get; set; }
    public int ratingDiff { get; set; }
}

public class Perf
{
    public string icon { get; set; }
    public string name { get; set; }
}

public class Status
{
    public int id { get; set; }
    public string name { get; set; }
}

public class TimeControl
{
    public string type { get; set; }
    public int limit { get; set; }
    public int increment { get; set; }
    public string show { get; set; }
}

public class Variant
{
    public string key { get; set; }
    public string name { get; set; }
    public string @short { get; set; }
}

public class Challenge
{
    public string id { get; set; }
    public string url { get; set; }
    public string status { get; set; }
    public Compat compat { get; set; }
    public Challenger challenger { get; set; }
    public DestUser destUser { get; set; }
    public Variant variant { get; set; }
    public bool rated { get; set; }
    public TimeControl timeControl { get; set; }
    public string color { get; set; }
    public string finalColor { get; set; }
    public string speed { get; set; }
    public Perf perf { get; set; }
    public string declineReason { get; set; }
    public string declineReasonKey { get; set; }
}

public class Challenger
{
    public string id { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public int rating { get; set; }
    public bool patron { get; set; }
    public bool online { get; set; }
    public int lag { get; set; }
}

public class Compat
{
    public bool bot { get; set; }
    public bool board { get; set; }
}


public class DestUser
{
    public string id { get; set; }
    public string name { get; set; }
    public object title { get; set; }
    public int rating { get; set; }
    public bool provisional { get; set; }
    public bool online { get; set; }
    public int lag { get; set; }
}